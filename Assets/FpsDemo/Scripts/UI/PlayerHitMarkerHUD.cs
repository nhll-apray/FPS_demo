using FpsDemo.Combat;
using FpsDemo.Game;
using FpsDemo.Player;
using UnityEngine;
using UnityEngine.UI;

namespace FpsDemo.UI
{
    public class PlayerHitMarkerHUD : MonoBehaviour
    {
        [SerializeField] private CanvasGroup markerGroup;
        [SerializeField] private RectTransform markerRoot;
        [SerializeField] private Graphic[] lineGraphics;

        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color criticalColor = new Color(1f, 0.12f, 0.08f, 1f);

        [SerializeField] private float damageForMinLength = 10f;
        [SerializeField] private float damageForMaxLength = 100f;
        [SerializeField] private float minLineLength = 28f;
        [SerializeField] private float maxLineLength = 76f;
        [SerializeField] private float lineThickness = 7f;
        [SerializeField] private float centerGap = 10f;
        [SerializeField] private float visibleDuration = 0.16f;
        [SerializeField] private float fadeDuration = 0.18f;
        [SerializeField] private float lengthGrowSpeed = 320f;
        [SerializeField] private float hitScale = 1.08f;
        [SerializeField] private float scaleRecoverSpeed = 16f;

        private static readonly Vector2[] LineDirections =
        {
            new Vector2(-1f, 1f).normalized,
            new Vector2(1f, 1f).normalized,
            new Vector2(-1f, -1f).normalized,
            new Vector2(1f, -1f).normalized
        };

        private static readonly float[] LineAngles =
        {
            135f,
            45f,
            -135f,
            -45f
        };

        private PlayerEntity _player;
        private float _accumulatedDamage;
        private float _visibleUntil;
        private float _currentAlpha;
        private float _currentLength;
        private float _targetLength;
        private Vector3 _baseScale;
        private bool _isShowing;
        private bool _isCriticalHit;

        private void Awake()
        {
            AutoAssignReferences();
            _baseScale = markerRoot != null ? markerRoot.localScale : Vector3.one;
            HideImmediate();
        }

        private void Start()
        {
            ResolvePlayer();
        }

        private void OnEnable()
        {
            EventManager.AddListener<DamageDealtEvent>(OnDamageDealt);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<DamageDealtEvent>(OnDamageDealt);
        }

        private void Update()
        {
            if (!_isShowing)
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            _currentLength = Mathf.MoveTowards(_currentLength, _targetLength, lengthGrowSpeed * deltaTime);
            ApplyLength(_currentLength);
            RecoverScale(deltaTime);

            if (Time.time <= _visibleUntil)
            {
                SetAlpha(1f);
                return;
            }

            float fadeProgress = fadeDuration <= 0f
                ? 1f
                : Mathf.Clamp01((Time.time - _visibleUntil) / fadeDuration);

            SetAlpha(1f - fadeProgress);

            if (fadeProgress >= 1f)
            {
                ResetRound();
                HideImmediate();
            }
        }

        private void OnDamageDealt(DamageDealtEvent eventData)
        {
            DamageResult result = eventData.DamageResult;
            if (!IsPlayerDamage(result))
            {
                return;
            }

            if (!_isShowing)
            {
                ResetRound();
                _currentLength = minLineLength;
            }

            _accumulatedDamage += result.DamageApplied;
            _isCriticalHit = result.IsHeadshot;
            _targetLength = CalculateLength(_accumulatedDamage);
            _visibleUntil = Time.time + visibleDuration;
            _isShowing = true;

            SetAlpha(1f);
            PunchScale();
        }

        private bool IsPlayerDamage(DamageResult result)
        {
            if (result.DamageApplied <= 0 || result.Attacker == null)
            {
                return false;
            }

            if (_player == null)
            {
                ResolvePlayer();
            }

            return _player != null && result.Attacker == _player.gameObject;
        }

        private void ResolvePlayer()
        {
            _player = GameManager.Instance != null
                ? GameManager.Instance.CurrentPlayer
                : null;

            if (_player == null)
            {
                _player = Object.FindFirstObjectByType<PlayerEntity>();
            }
        }

        private float CalculateLength(float damage)
        {
            float t = Mathf.InverseLerp(damageForMinLength, damageForMaxLength, damage);
            return Mathf.Lerp(minLineLength, maxLineLength, t);
        }

        private void ApplyLength(float length)
        {
            if (lineGraphics == null)
            {
                return;
            }

            for (int i = 0; i < lineGraphics.Length; i++)
            {
                Graphic lineGraphic = lineGraphics[i];
                if (lineGraphic == null)
                {
                    continue;
                }

                RectTransform lineRect = lineGraphic.rectTransform;
                int layoutIndex = Mathf.Min(i, LineDirections.Length - 1);
                lineRect.pivot = new Vector2(0f, 0.5f);
                lineRect.anchoredPosition = LineDirections[layoutIndex] * centerGap;
                lineRect.localRotation = Quaternion.Euler(0f, 0f, LineAngles[layoutIndex]);
                lineRect.sizeDelta = new Vector2(length, lineThickness);
            }
        }

        private void ApplyColor(Color color)
        {
            if (lineGraphics == null)
            {
                return;
            }

            for (int i = 0; i < lineGraphics.Length; i++)
            {
                if (lineGraphics[i] != null)
                {
                    lineGraphics[i].color = color;
                }
            }
        }

        private void SetAlpha(float alpha)
        {
            _currentAlpha = Mathf.Clamp01(alpha);

            if (markerGroup != null)
            {
                markerGroup.alpha = _currentAlpha;
            }

            Color color = _isCriticalHit ? criticalColor : normalColor;
            color.a = markerGroup != null ? 1f : _currentAlpha;
            ApplyColor(color);
        }

        private void PunchScale()
        {
            if (markerRoot != null)
            {
                markerRoot.localScale = _baseScale * hitScale;
            }
        }

        private void RecoverScale(float deltaTime)
        {
            if (markerRoot == null)
            {
                return;
            }

            markerRoot.localScale = Vector3.Lerp(
                markerRoot.localScale,
                _baseScale,
                1f - Mathf.Exp(-scaleRecoverSpeed * deltaTime));
        }

        private void ResetRound()
        {
            _accumulatedDamage = 0f;
            _isCriticalHit = false;
            _visibleUntil = 0f;
            _targetLength = minLineLength;
            _isShowing = false;
        }

        private void HideImmediate()
        {
            SetAlpha(0f);
            ApplyLength(minLineLength);

            if (markerRoot != null)
            {
                markerRoot.localScale = _baseScale;
            }
        }

        private void AutoAssignReferences()
        {
            if (markerRoot == null)
            {
                markerRoot = transform as RectTransform;
            }

            if (markerGroup == null)
            {
                markerGroup = GetComponent<CanvasGroup>();
            }

            if (lineGraphics == null || lineGraphics.Length == 0)
            {
                lineGraphics = GetComponentsInChildren<Graphic>(true);
            }
        }
    }
}
