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
        [SerializeField, Range(1f, 3f)]
        private float _maxIdleSpeed = 2;
        [SerializeField] private float fadeDuration = 1.0f;


        [SerializeField] private float _maxTilt = 5;
        [SerializeField] private float _tiltSpeed = 20;

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

        public void FadeOut ()
        {
            StartCoroutine(FadeOutRoutine());
        }

        private IEnumerator FadeOutRoutine ()
        {
            List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
            if (spriteRenderers.Count > 0)
            {
                float elapsedTime = 0;
                List<Color> originalColors = new List<Color>();

                // Store the original colors
                foreach (var renderer in spriteRenderers)
                {
                    originalColors.Add(renderer.color);
                }

                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float alpha = Mathf.Clamp01(1.0f - (elapsedTime / fadeDuration));

                    for (int i = 0; i < spriteRenderers.Count; i++)
                    {
                        Color newColor = originalColors[i];
                        newColor.a = alpha;
                        spriteRenderers[i].color = newColor;
                    }

                    yield return null;
                }

                // Ensure they all end fully transparent
                for (int i = 0; i < spriteRenderers.Count; i++)
                {
                    Color transparentColor = originalColors[i];
                    transparentColor.a = 0;
                    spriteRenderers[i].color = transparentColor;
                }
            }
            else
            {
                Debug.LogWarning("No SpriteRenderers found on the GameObject.");
            }
        }

    }
}