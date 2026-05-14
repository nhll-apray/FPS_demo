using System;
using System.Numerics;
using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public class HitscanWeapon : WeaponBase
    {
        [SerializeField] private HitscanWeaponData weaponData;
        public override WeaponData WeaponData => weaponData;

        enum WeaponState
        {
            Idle,
            Firing,
            Reloading
        }
        
        private WeaponState _currentState;

        private float _nextFireTime;

        private void Update()
        {
            if (_currentState == WeaponState.Firing)
            {
                TryFire();
            }
        }

        public override void StartFire()
        {
            base.StartFire();
            _currentState = WeaponState.Firing;
            _nextFireTime = Time.time;
            TryFire();
        }

        public override void StopFire()
        {
            base.StopFire();
            _currentState = WeaponState.Idle;
        }

        private void TryFire()
        {
            if (aimProvider == null) return;
            if (Time.time >= _nextFireTime && currentAmmo > 0)
            {
                Fire();
                _nextFireTime = Time.time + weaponData.FireInterval;
            }
        }

        private void Fire()
        {
            currentAmmo--;
            audioSource.clip = weaponData.ShootSound;
            audioSource.Play();
            
            GameObject aimTarget = GetAimTarget();
            if (aimTarget != null)
            {
                IDamageable damageable = aimTarget.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(new DamageInfo(weaponData.Damage));
                }
            }
        }
        
        public GameObject GetAimTarget()
        {
            if (Physics.Raycast(aimProvider.GetAimRay(), out RaycastHit hit, weaponData.Range, LayerMask.GetMask("Enemy")))
            {
                return hit.collider.gameObject;
            }
            return null;
        }
        
        public override void Reload()
        {
            base.Reload();
        }
    }
}