using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FpsDemo.Weapon;

namespace FpsDemo.UI
{
    public class WeaponHUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text ammoText;
        [SerializeField] private Image weaponImage;

        private WeaponInventory _weaponInventory;
        

        private void OnDestroy()
        {
            UnbindCurrentWeapon();
        }

        public void Bind(WeaponInventory weaponInventory)
        {
            UnbindCurrentWeapon();
            _weaponInventory = weaponInventory;

            if (_weaponInventory != null && _weaponInventory.CurrentWeapon != null)
            {
                WeaponBase currentWeapon = _weaponInventory.CurrentWeapon;
                weaponImage.sprite = currentWeapon.WeaponConfig.Icon;
                currentWeapon.OnAmmoChange += OnAmmoChanged;
                RefreshAmmo(currentWeapon.CurrentAmmo, currentWeapon.WeaponConfig.MaxAmmo);
            }
        }

        private void UnbindCurrentWeapon()
        {
            if (_weaponInventory != null && _weaponInventory.CurrentWeapon != null)
            {
                _weaponInventory.CurrentWeapon.OnAmmoChange -= OnAmmoChanged;
            }
        }

        private void OnAmmoChanged(int preAmmo, int currentAmmo)
        {
            RefreshAmmo(currentAmmo, _weaponInventory.CurrentWeapon.WeaponConfig.MaxAmmo);
        }
        
        private void RefreshAmmo(int currentAmmo, int maxAmmo)
        {
            if (ammoText != null)
                ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }
    }
}
