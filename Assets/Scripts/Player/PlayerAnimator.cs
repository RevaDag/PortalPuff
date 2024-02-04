using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TarodevController
{

    public class PlayerAnimator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Animator _anim;


        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.5f;


        [Header("Particles")][SerializeField] private ParticleSystem _jumpParticles;
        [SerializeField] private ParticleSystem _launchParticles;
        [SerializeField] private ParticleSystem _moveParticles;
        [SerializeField] private ParticleSystem _landParticles;

        [Header("Audio Clips")]
        [SerializeField]
        private AudioClip[] _footsteps;

        private AudioSource _source;
        private IPlayerController _player;
        private bool _grounded;
        private ParticleSystem.MinMaxGradient _currentGradient;

        private void Awake ()
        {
            _source = GetComponent<AudioSource>();
            _player = GetComponentInParent<IPlayerController>();
        }

        private void OnEnable ()
        {
            _player.Jumped += OnJumped;
            _player.GroundedChanged += OnGroundedChanged;
            _player.Died += OnDeath;

            _moveParticles.Play();
        }

        private void OnDisable ()
        {
            _player.Jumped -= OnJumped;
            _player.GroundedChanged -= OnGroundedChanged;

            _moveParticles.Stop();
        }

        private void Update ()
        {
            if (_player == null) return;

            DetectGroundColor();

            HandleIdleSpeed();

            WalkingAnim();

            HandleFlip();
        }

        public void ActivateAnimation ( bool _isActive )
        {
            _anim.enabled = _isActive;
        }

        public void HandleFlip ()
        {
            if ((_player.FrameInput.x > 0 && transform.localScale.x < 0) || (_player.FrameInput.x < 0 && transform.localScale.x > 0))
            {
                Vector3 playerScale = transform.localScale;
                playerScale.x *= -1;
                transform.localScale = playerScale;
            }
        }


        private void HandleIdleSpeed ()
        {
            var inputStrength = Mathf.Abs(_player.FrameInput.x);
            _moveParticles.transform.localScale = Vector3.MoveTowards(_moveParticles.transform.localScale, Vector3.one * inputStrength, 2 * Time.deltaTime);
        }



        private void WalkingAnim ()
        {
            if (_player.FrameInput.x != 0)
            {
                _anim.SetBool("IsWalking", true);
            }
            else
                _anim.SetBool("IsWalking", false);
        }

        private void OnJumped ()
        {
            _anim.SetTrigger("Jump");
            _anim.SetBool("IsGrounded", false);


            if (_grounded) // Avoid coyote
            {
                SetColor(_jumpParticles);
                SetColor(_launchParticles);
                _jumpParticles.Play();
            }
        }

        private void OnGroundedChanged ( bool grounded, float impact )
        {
            _grounded = grounded;

            if (grounded)
            {
                DetectGroundColor();
                SetColor(_landParticles);

                _anim.ResetTrigger("Jump");
                _anim.SetBool("IsGrounded", true);
                _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
                _moveParticles.Play();

                _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
                _landParticles.Play();
            }
            else
            {
                _moveParticles.Stop();
            }
        }

        private void OnDeath ()
        {
            _anim.SetBool("IsDead", true);
            _anim.SetTrigger("Hurt");

        }

        private void DetectGroundColor ()
        {
            var hit = Physics2D.Raycast(transform.position, Vector3.down, 2);

            if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r)) return;
            var color = r.color;
            _currentGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
            SetColor(_moveParticles);
        }

        private void SetColor ( ParticleSystem ps )
        {
            var main = ps.main;
            main.startColor = _currentGradient;
        }

        #region Fade In & Out


        public void Fade ( int startAlpha, int targetAlpha )
        {
            _anim.enabled = false; // Assuming _anim is a reference to an Animator component that should be disabled during the fade
            StartCoroutine(FadeRoutine(startAlpha, targetAlpha));
        }

        private IEnumerator FadeRoutine ( int startAlpha, int targetAlpha )
        {
            float elapsedTime = 0;
            List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
            if (spriteRenderers.Count == 0)
            {
                Debug.LogWarning("No SpriteRenderers found on the GameObject.");
                yield break; // Exit if no sprite renderers found
            }

            // Loop over the duration
            while (elapsedTime < fadeDuration)
            {
                foreach (var renderer in spriteRenderers)
                {
                    Color currentColor = renderer.color;
                    Color targetColor = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha); // Target is opaque
                    renderer.color = Color.Lerp(new Color(currentColor.r, currentColor.g, currentColor.b, startAlpha), targetColor, elapsedTime / fadeDuration);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure all sprite renderers are fully opaque at the end
            foreach (var renderer in spriteRenderers)
            {
                Color currentColor = renderer.color;
                renderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
            }
        }

        #endregion
    }
}