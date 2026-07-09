using FpsDemo.Weapon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FpsDemo.UI
{
    public class GrenadeCooldownHUD : MonoBehaviour
    {
        [SerializeField] private GrenadeAltFire grenadeAltFire;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image cooldownFillImage;
        [SerializeField] private TMP_Text cooldownText;
        [SerializeField] private Color readyIconColor = Color.white;
        [SerializeField] private Color cooldownIconColor = new Color(0.45f, 0.48f, 0.5f, 1f);

        private WeaponInventory _weaponInventory;
        private WeaponBase _boundWeapon;

        private void Awake()
        {
            AutoAssignReferences();
            ConfigureCooldownFill();
            Refresh();
        }

        private void Update()
        {
            RefreshAltFireBinding();
            Refresh();
        }

        public void Bind(GrenadeAltFire altFire)
        {
            _weaponInventory = null;
            _boundWeapon = null;
            grenadeAltFire = altFire;
            Refresh();
        }

        public void Bind(WeaponInventory weaponInventory)
        {
            _weaponInventory = weaponInventory;
            _boundWeapon = null;
            RefreshAltFireBinding();
            Refresh();
        }

        private void RefreshAltFireBinding()
        {
            if (_weaponInventory == null)
            {
                return;
            }

            WeaponBase currentWeapon = _weaponInventory.CurrentWeapon;
            if (_boundWeapon == currentWeapon)
            {
                return;
            }

            _boundWeapon = currentWeapon;
            grenadeAltFire = currentWeapon != null ? currentWeapon.GetComponent<GrenadeAltFire>() : null;
        }

        private void Refresh()
        {
            float remaining = grenadeAltFire != null ? grenadeAltFire.CooldownRemaining : 0f;
            float normalized = grenadeAltFire != null ? grenadeAltFire.CooldownNormalized : 0f;
            bool isCoolingDown = remaining > 0.01f;

            if (cooldownFillImage != null)
            {
                cooldownFillImage.fillAmount = normalized;
                cooldownFillImage.enabled = normalized > 0f;
            }

            if (cooldownText != null)
            {
                cooldownText.text = remaining > 0.05f
                    ? Mathf.CeilToInt(remaining).ToString()
                    : string.Empty;
            }

            if (iconImage != null)
            {
                iconImage.color = isCoolingDown ? cooldownIconColor : readyIconColor;
            }
        }

        private void ConfigureCooldownFill()
        {
            if (cooldownFillImage == null)
            {
                return;
            }

            cooldownFillImage.type = Image.Type.Filled;
            cooldownFillImage.fillMethod = Image.FillMethod.Radial360;
            cooldownFillImage.fillOrigin = (int)Image.Origin360.Top;
            cooldownFillImage.fillClockwise = false;
        }

        private void AutoAssignReferences()
        {
            if (iconImage == null)
            {
                iconImage = FindChildComponentByName<Image>(transform, "Icon");
            }

            if (cooldownFillImage == null)
            {
                cooldownFillImage = FindChildComponentByName<Image>(transform, "CooldownFill");
            }

            if (cooldownText == null)
            {
                cooldownText = FindChildComponentByName<TMP_Text>(transform, "CooldownText");
            }
        }

        private static T FindChildComponentByName<T>(Transform root, string childName) where T : Component
        {
            if (root == null)
            {
                return null;
            }

            foreach (T component in root.GetComponentsInChildren<T>(true))
            {
                if (component.name == childName)
                {
                    return component;
                }
            }

            return null;
        }
    }
}
