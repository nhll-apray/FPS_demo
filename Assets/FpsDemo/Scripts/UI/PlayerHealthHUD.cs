using FpsDemo.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FpsDemo.UI
{
    public class PlayerHealthHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform viewRoot;
        [SerializeField] private Image fillImage;
        [SerializeField] private TMP_Text valueText;

        private Health _health;

        public void Bind(Health health)
        {
            if (_health == health)
            {
                Refresh();
                return;
            }

            Unbind();

            _health = health;
            if (_health != null)
            {
                _health.OnHealthChanged += OnHealthChanged;
            }

            Refresh();
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void Awake()
        {
            AutoAssignReferences();
            ConfigureFillImage();
        }

        private void Unbind()
        {
            if (_health == null)
            {
                return;
            }

            _health.OnHealthChanged -= OnHealthChanged;
            _health = null;
        }

        private void OnHealthChanged(int currentHealth, int maxHealth)
        {
            Refresh(currentHealth, maxHealth);
        }

        private void Refresh()
        {
            if (_health == null)
            {
                Refresh(0, 0);
                return;
            }

            Refresh(_health.CurrentHealth, _health.MaxHealth);
        }

        private void Refresh(int currentHealth, int maxHealth)
        {
            int displayCurrent = Mathf.Max(0, currentHealth);
            int displayMax = Mathf.Max(0, maxHealth);
            float normalized = displayMax <= 0 ? 0f : Mathf.Clamp01((float)displayCurrent / displayMax);

            if (viewRoot != null)
            {
                viewRoot.gameObject.SetActive(displayMax > 0);
            }

            if (fillImage != null)
            {
                fillImage.fillAmount = normalized;
            }

            if (valueText != null)
            {
                valueText.text = $"{displayCurrent} / {displayMax}";
            }
        }

        private void AutoAssignReferences()
        {
            if (viewRoot == null)
            {
                viewRoot = transform.name == nameof(PlayerHealthHUD)
                    ? transform as RectTransform
                    : transform.Find(nameof(PlayerHealthHUD)) as RectTransform;
            }

            Transform searchRoot = viewRoot != null ? viewRoot : transform;

            if (fillImage == null)
            {
                fillImage = FindChildComponentByName<Image>(searchRoot, "BarFill");
            }

            if (valueText == null)
            {
                valueText = FindChildComponentByName<TMP_Text>(searchRoot, "Value");
            }
        }

        private static T FindChildComponentByName<T>(Transform root, string childName) where T : Component
        {
            if (root == null)
            {
                return null;
            }

            T[] components = root.GetComponentsInChildren<T>(true);
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].name == childName)
                {
                    return components[i];
                }
            }

            return null;
        }

        private void ConfigureFillImage()
        {
            if (fillImage == null)
            {
                return;
            }

            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
    }
}
