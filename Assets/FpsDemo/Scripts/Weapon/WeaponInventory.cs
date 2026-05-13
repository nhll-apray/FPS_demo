using System;
using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField]
        private WeaponBase currentWeapon;
        public WeaponBase CurrentWeapon
        {
            get => currentWeapon;
            set
            {
                if (currentWeapon != null)
                {
                    currentWeapon.SetOwnerInfo(null, null);
                }
                currentWeapon = value;
                if (currentWeapon != null)
                {
                    currentWeapon.SetOwnerInfo(gameObject, _aimProvider);
                }
            }
        }
        
        private IAimProvider _aimProvider;
        

        private void Awake()
        {
            _aimProvider = GetComponent<IAimProvider>();
        }

        private void Start()
        {
            CurrentWeapon = currentWeapon;
        }

        public void StartFire()
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.StartFire();
            }
        }

        public void StopFire()
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.StopFire();
            }
        }

        public void Reload()
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.Reload();
            }
        }
    }
}