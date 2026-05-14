using FpsDemo.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FpsDemo.Enemy
{
    public class EnemyNameplate : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Transform followTarget;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image fillImage;
        [SerializeField] private TMP_Text nameText;

        [SerializeField] private GameObject healthBarRoot;
        
        [SerializeField] private string displayName = "Enemy";
        
        [SerializeField] private float visibleDurationAfterDamage = 3f;
        [SerializeField] private float maxVisibleDistance = 60f;
        
        
        [SerializeField] private float desiredScreenHeightPixels = 100f;
        
        [SerializeField] private float constantSizeDistance = 20f;
        
        [SerializeField] private float minScreenHeightPixels = 28f;
        [SerializeField] private float farShrinkPower = 0.25f;

        private RectTransform _rectTransform;
        private float _referenceRectHeight;
        private float _visibleUntilTime = -1f;
        private int _lastHealth;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            if (health == null)
                health = GetComponentInParent<Health>();

            if (followTarget == null && health != null)
                followTarget = health.transform;

            if (playerCamera == null)
                playerCamera = Camera.main;

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            
            if (nameText != null)
                nameText.text = displayName;

            if (_rectTransform != null)
            {
                _referenceRectHeight = Mathf.Max(1f, _rectTransform.rect.height);

                // 建议根节点 pivot 使用底部中心，避免缩放时 UI 往下压到敌人模型。
                _rectTransform.pivot = new Vector2(0.5f, 0f);
            }
            else
            {
                _referenceRectHeight = 1f;
            }

            if (fillImage != null)
            {
                // 防止 Fill Image 忘记设置成 Filled。
                fillImage.type = Image.Type.Filled;
                fillImage.fillMethod = Image.FillMethod.Horizontal;
                fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            }

            if (health != null)
            {
                _lastHealth = health.CurrentHealth;
                RefreshHealthBar(health.CurrentHealth, health.MaxHealth);
            }

            HideImmediate();
        }

        private void OnEnable()
        {
            if (health == null)
                return;

            health.onHealthChanged += OnHealthChanged;
            health.died += OnDied;

            RefreshHealthBar(health.CurrentHealth, health.MaxHealth);
        }

        private void OnDisable()
        {
            if (health == null)
                return;

            health.onHealthChanged -= OnHealthChanged;
            health.died -= OnDied;
        }

        private void LateUpdate()
        {
            if (playerCamera == null || followTarget == null)
                return;
            

            float distance = Vector3.Distance(playerCamera.transform.position, followTarget.position);

            UpdatePosition(followTarget.position);
            UpdateRotation();
            UpdateScale(distance);
            UpdateVisibility(distance);
        }

        private void OnHealthChanged(int currentHealth, int maxHealth)
        {
            RefreshHealthBar(currentHealth, maxHealth);

            bool tookDamage = currentHealth < _lastHealth;

            if (tookDamage && currentHealth > 0)
            {
                ShowForDuration();
            }

            _lastHealth = currentHealth;
        }

        private void OnDied()
        {
            HideImmediate();
        }

        private void RefreshHealthBar(int currentHealth, int maxHealth)
        {
            if (fillImage == null)
                return;

            float normalized = maxHealth <= 0 ? 0f : (float)currentHealth / maxHealth;

            fillImage.fillAmount = Mathf.Clamp01(normalized);
        }

        private void ShowForDuration()
        {
            _visibleUntilTime = Time.time + visibleDurationAfterDamage;
            SetVisible(true);
        }

        private void HideImmediate()
        {
            _visibleUntilTime = -1f;
            SetVisible(false);
        }

        private void UpdatePosition(Vector3 anchorPosition)
        {
            transform.position = anchorPosition;
        }

        private void UpdateRotation()
        {
            Vector3 directionToNameplate = transform.position - playerCamera.transform.position;
            directionToNameplate.y = 0f;

            if (directionToNameplate.sqrMagnitude <= 0.0001f)
                return;

            Vector3 forward = directionToNameplate.normalized;

            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        private void UpdateScale(float distance)
        {
            float targetScreenHeightPixels = GetTargetScreenHeightPixels(distance);

            float desiredWorldHeight = ScreenPixelsToWorldHeight(distance, targetScreenHeightPixels);

            float scale = desiredWorldHeight / _referenceRectHeight;

            transform.localScale = Vector3.one * scale;
        }

        private float GetTargetScreenHeightPixels(float distance)
        {
            float startDistance = Mathf.Max(0.01f, constantSizeDistance);

            if (distance <= startDistance)
                return desiredScreenHeightPixels;

            float shrinkFactor = Mathf.Pow(startDistance / distance, farShrinkPower);

            float targetPixels = desiredScreenHeightPixels * shrinkFactor;

            return Mathf.Max(minScreenHeightPixels, targetPixels);
        }

        private float ScreenPixelsToWorldHeight(float distance, float pixels)
        {
            if (Screen.height <= 0)
                return 0f;

            if (playerCamera.orthographic)
            {
                float worldScreenHeight = playerCamera.orthographicSize * 2f;
                return worldScreenHeight * pixels / Screen.height;
            }

            float worldScreenHeightAtDistance = 2f * distance * Mathf.Tan(playerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            return worldScreenHeightAtDistance * pixels / Screen.height;
        }

        private void UpdateVisibility(float distance)
        {
            if (health != null && health.IsDead)
            {
                SetVisible(false);
                return;
            }

            if (distance > maxVisibleDistance)
            {
                SetVisible(false);
                return;
            }

            bool shouldShowBecauseRecentlyDamaged = _visibleUntilTime > 0f && Time.time <= _visibleUntilTime;

            SetVisible(shouldShowBecauseRecentlyDamaged);
        }

        private void SetVisible(bool visible)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            SetActiveIfNeeded(healthBarRoot, visible);
            SetActiveIfNeeded(nameText.gameObject, visible);
        }

        private void SetActiveIfNeeded(GameObject target, bool active)
        {
            if (target == null)
                return;
            
            if (target == gameObject)
                return;

            if (target.activeSelf != active)
                target.SetActive(active);
        }
    }
}