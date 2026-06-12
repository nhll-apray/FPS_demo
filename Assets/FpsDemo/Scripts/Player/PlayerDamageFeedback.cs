using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Player
{
    public class PlayerDamageFeedback : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private PlayerCameraController cameraController;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hurtSound;
        [SerializeField] private float hurtSoundVolume = 0.75f;
        [SerializeField] private float damageForFullShake = 20f;

        private int _lastHealth;

        private void Awake()
        {
            if (health == null)
            {
                health = GetComponent<Health>();
            }

            if (cameraController == null)
            {
                cameraController = GetComponent<PlayerCameraController>();
            }

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        private void OnEnable()
        {
            if (health != null)
            {
                _lastHealth = health.CurrentHealth;
                health.OnHealthChanged += OnHealthChanged;
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.OnHealthChanged -= OnHealthChanged;
            }
        }

        private void OnHealthChanged(int currentHealth, int maxHealth)
        {
            int damageTaken = Mathf.Max(0, _lastHealth - currentHealth);
            _lastHealth = currentHealth;

            if (damageTaken <= 0)
            {
                return;
            }

            float intensity = damageForFullShake <= 0f ? 1f : Mathf.Clamp01(damageTaken / damageForFullShake);

            cameraController?.PlayDamageShake(intensity);
            PlayHurtSound();
        }

        private void PlayHurtSound()
        {
            if (audioSource == null || hurtSound == null)
            {
                return;
            }

            audioSource.PlayOneShot(hurtSound, hurtSoundVolume);
        }
    }
}
