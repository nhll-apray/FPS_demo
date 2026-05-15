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
        

        private void OnDestory()
        {
            if (_weaponInventory != null && _weaponInventory.CurrentWeapon != null)
            {
                _weaponInventory.CurrentWeapon.OnAmmoChange -= OnAmmoChanged;
            }
        }

        public void Bind(WeaponInventory weaponInventory)
        {
            _weaponInventory = weaponInventory;
            weaponImage.sprite = weaponInventory.CurrentWeapon.WeaponData.Icon;
            if (_weaponInventory != null && _weaponInventory.CurrentWeapon != null)
            {
                _weaponInventory.CurrentWeapon.OnAmmoChange += OnAmmoChanged;
            }
        }

        private void OnAmmoChanged(int preAmmo, int currentAmmo)
        {
            RefreshAmmo(currentAmmo, _weaponInventory.CurrentWeapon.WeaponData.MaxAmmo);
        }
        
        private void RefreshAmmo(int currentAmmo, int maxAmmo)
        {
            if (ammoText != null)
                ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }
    }
}