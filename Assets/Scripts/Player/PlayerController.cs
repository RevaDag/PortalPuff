using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using CandyCoded.HapticFeedback;
using System.Collections;

namespace TarodevController
{

    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {

        [Header("Actions")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference interactAction;

        [Header("References")]
        [SerializeField] private ScriptableStats _stats;
        [SerializeField] private ScriptableStats alternativeStats;
        [SerializeField] private Collider2D _groundCheck;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private PlayerDuplicationsManager _duplicationsManager;

        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private Vector2 _environmentVelocity;
        private bool _cachedQueryStartInColliders;
        private bool gatherInput;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        public event Action Died;

        #endregion

        private float _time;

        private GameObject currentInteractable = null;
        private bool _inSpringJump;



        private void Awake ()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Start ()
        {
            _duplicationsManager?.AddNewPlayer(this.gameObject);
            gatherInput = true;
        }

        private void Update ()
        {
            _time += Time.deltaTime;
            if (gatherInput)
                GatherInput();
            HandleInteraction();
        }

        #region Gamepad Actions
        private bool GamepadJumpDown => jumpAction.action.triggered;
        private bool GamepadJumpHeld => jumpAction.action.ReadValue<float>() > 0.1f;
        private bool GamepadInteraction => interactAction.action.triggered;


        #endregion

        private void GatherInput ()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C) || GamepadJumpDown,
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C) || GamepadJumpHeld,
                KeyboardMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
                JoystickMove = moveAction.action.ReadValue<Vector2>(),
                Move = _frameInput.KeyboardMove + _frameInput.JoystickMove
            };


            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
        }

        private void FixedUpdate ()
        {
            CheckCollisions();

            HandleJump();
            HandleDirection();
            HandleGravity();

            ApplyMovement();
        }

        public void ActivateGatherInput ( bool isActive )
        {
            gatherInput = isActive;
            _frameInput.Move.x = 0;
            _frameInput.Move.y = 0;
        }

        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions ()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling   
            bool groundHit = _groundCheck.IsTouchingLayers(_groundLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

            // Hit a Ceiling
            if (ceilingHit)
            {
                _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);
                _endedJumpEarly = true;
            }


            // Landed on the Ground
            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            // Left the Ground
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }


        private void OnTriggerEnter2D ( Collider2D other )
        {
            if (other.CompareTag("Interactable")) // Make sure to set this tag on your interactable objects
            {
                currentInteractable = other.gameObject;
            }
        }

        private void OnTriggerExit2D ( Collider2D other )
        {
            if (other.gameObject == currentInteractable)
            {
                currentInteractable = null;
            }
        }

        #endregion

        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump ()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y != 0) _endedJumpEarly = true;
            if (_inSpringJump) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote) ExecuteJump(1);

            _jumpToConsume = false;
        }

        public void ExecuteJump ( float jumpMultiply )
        {
            if (jumpMultiply > 1)
            {
                StopCoroutine("SpringJump"); // Stop existing SpringJump coroutine
                StartCoroutine(SpringJump());
            }

            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower * jumpMultiply * transform.localScale.x;

            HapticFeedback.LightFeedback();
            Jumped?.Invoke();
        }

        public IEnumerator SpringJump ()
        {
            _inSpringJump = true;
            yield return new WaitForSeconds(1); // Consider adjusting or making this duration configurable
            _inSpringJump = false;
        }


        #endregion

        #region Horizontal

        private void HandleDirection ()
        {
            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity ()
        {
            if (_grounded && _frameVelocity.y == 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y != 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        public async void FlipGravity ()
        {
            // Switch between the Scriptable Stats files
            ScriptableStats tempStats = _stats;
            _stats = alternativeStats;
            alternativeStats = tempStats;

            await Task.Delay(300);

            Vector3 theScale = transform.localScale;
            theScale.y *= -1; // Flip the player's sprite vertically
            transform.localScale = theScale;

            if (transform.localScale.y < 0f)
            {
                Vector3 newPosition = transform.position;
                newPosition.y += 1;
                transform.position = newPosition;
            }
            else
            {
                Vector3 newPosition = transform.position;
                newPosition.y -= 1;
                transform.position = newPosition;
            }
        }

        #endregion

        public void ApplyEnvironemntVelocity ( Vector2 _velocity )
        {
            _environmentVelocity = _velocity;
        }

        private void ApplyMovement () => _rb.velocity = _frameVelocity + _environmentVelocity;

        private void HandleInteraction ()
        {
            if ((Input.GetKeyDown(KeyCode.E) || GamepadInteraction) && currentInteractable != null)
            {
                IInteractable interactable = currentInteractable.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    HapticFeedback.MediumFeedback();
                    interactable.Interact(gameObject);
                }
            }
        }

        public void Freeze ()
        {
            _rb.velocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public void Unfreeze ()
        {
            _rb.constraints = RigidbodyConstraints2D.None;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public async void Die ()
        {
            if (!gatherInput) return;

            _rb.velocity = Vector2.zero;
            ActivateGatherInput(false);
            HapticFeedback.HeavyFeedback();

            Died?.Invoke();
            _duplicationsManager.RemovePlayer(this.gameObject);
            await Task.Delay(1000);

            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnValidate ()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 KeyboardMove;
        public Vector2 JoystickMove;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;

        public event Action Died;
        public Vector2 FrameInput { get; }
    }
}