using FpsDemo.Combat;
using FpsDemo.Player;
using UnityEngine;

namespace FpsDemo.UI
{
    public class PlayerDamageOverlayHUD : MonoBehaviour
    {
        [SerializeField] private CanvasGroup overlayGroup;
        [SerializeField] private float minFlashAlpha = 0.35f;
        [SerializeField] private float maxFlashAlpha = 0.65f;
        [SerializeField] private float damageForFullFlash = 20f;
        [SerializeField] private float fadeSpeed = 2.8f;

        private Health _health;
        private float _currentAlpha;
        private int _lastHealth;
        private bool _isSubscribed;

        public void Bind(PlayerEntity player)
        {
            Unsubscribe();
            _health = player != null ? player.GetComponent<Health>() : null;
            if (_health != null)
            {
                _lastHealth = _health.CurrentHealth;
                Subscribe();
            }

            SetAlpha(0f);
        }

        private void Awake()
        {
            AutoAssignReferences();
            SetAlpha(0f);
        }
        

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Update()
        {
            if (_currentAlpha <= 0f)
            {
                return;
            }

            _currentAlpha = Mathf.MoveTowards(_currentAlpha, 0f, fadeSpeed * Time.deltaTime);
            SetAlpha(_currentAlpha);
        }

        private void Subscribe()
        {
            if (_isSubscribed || _health == null)
            {
                return;
            }

            _health.OnHealthChanged += OnHealthChanged;
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed || _health == null)
            {
                return;
            }

            _health.OnHealthChanged -= OnHealthChanged;
            _isSubscribed = false;
        }

        private void OnHealthChanged(int currentHealth, int maxHealth)
        {
            int damageTaken = Mathf.Max(0, _lastHealth - currentHealth);
            _lastHealth = currentHealth;

            if (damageTaken <= 0)
            {
                return;
            }

            float normalizedDamage = damageForFullFlash <= 0f
                ? 1f
                : Mathf.Clamp01(damageTaken / damageForFullFlash);
            float targetAlpha = Mathf.Lerp(minFlashAlpha, maxFlashAlpha, normalizedDamage);
            _currentAlpha = Mathf.Max(_currentAlpha, targetAlpha);
            SetAlpha(_currentAlpha);
        }

        private void AutoAssignReferences()
        {
            if (overlayGroup != null)
            {
                return;
            }

            Transform overlay = transform.Find("DamageOverlayHUD");
            overlayGroup = overlay != null ? overlay.GetComponent<CanvasGroup>() : GetComponentInChildren<CanvasGroup>(true);
        }

        private void SetAlpha(float alpha)
        {
            _currentAlpha = Mathf.Clamp01(alpha);
            if (overlayGroup != null)
            {
                overlayGroup.alpha = _currentAlpha;
            }
        }
    }
}
