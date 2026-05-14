using System;
using System.Collections;
using FpsDemo.Combat;
using UnityEngine;
using UnityEngine.Serialization;

namespace FpsDemo.Weapon
{
    public class HitscanWeapon : WeaponBase
    {
        public HitscanWeaponData hitscanWeaponData;
        public override WeaponData WeaponData => hitscanWeaponData;

        enum WeaponState
        {
            Idle,
            Firing,
            Reloading
        }

        private WeaponState _currentState;

        public bool IsReloading => _currentState == WeaponState.Reloading;
        public bool IsFiring => _currentState == WeaponState.Firing;

        private float _nextFireTime;

        // 关键：记录玩家当前是否还按着开火键。
        private bool _fireInputHeld;

        public event Action OnFiredStarted;
        public event Action OnFiredStoped;
        public event Action OnReloadStarted;
        public event Action OnReloadFinished;

        private void Update()
        {
            if (_currentState == WeaponState.Firing)
            {
                TryFire();
            }
        }

        public override void StartFire()
        {
            _fireInputHeld = true;

            if (IsReloading)
                return;

            if (_currentState == WeaponState.Firing)
                return;

            base.StartFire();

            _currentState = WeaponState.Firing;
            _nextFireTime = Time.time;

            TryFire();
        }

        public override void StopFire()
        {
            _fireInputHeld = false;
            
            if (IsReloading)
                return;

            if (_currentState != WeaponState.Firing)
                return;

            base.StopFire();

            StopFiringStateWithoutClearingInput();
        }

        private void TryFire()
        {
            if (aimProvider == null)
                return;

            if (_currentState != WeaponState.Firing)
                return;

            if (IsReloading)
                return;

            if (Time.time < _nextFireTime)
                return;

            if (currentAmmo > 0)
            {
                Fire();
                _nextFireTime = Time.time + hitscanWeaponData.FireInterval;
            }
            else
            {
                StopFiringStateWithoutClearingInput();
            }
        }

        private void Fire()
        {
            currentAmmo--;

            if (audioSource != null && hitscanWeaponData.ShootSound != null)
                audioSource.PlayOneShot(hitscanWeaponData.ShootSound);
            
            OnFiredStarted?.Invoke();

            GameObject aimTarget = GetAimTarget();

            if (aimTarget != null)
            {
                IDamageable damageable = aimTarget.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    DamageSystem.ApplyDamage(damageable, new DamageInfo(hitscanWeaponData.Damage, owner));
                }
            }
        }

        public GameObject GetAimTarget()
        {
            if (Physics.Raycast(aimProvider.GetAimRay(), out RaycastHit hit, hitscanWeaponData.Range, LayerMask.GetMask("Enemy")))
            {
                return hit.collider.gameObject;
            }
            return null;
        }

        public override void Reload()
        {
            if (_currentState == WeaponState.Reloading)
                return;

            if (currentAmmo >= hitscanWeaponData.MaxAmmo)
                return;

            StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            StopFiringStateWithoutClearingInput();

            _currentState = WeaponState.Reloading;

            if (audioSource != null && hitscanWeaponData.ReloadSound != null)
            {
                audioSource.clip = hitscanWeaponData.ReloadSound;
                audioSource.pitch = hitscanWeaponData.ReloadSound.length / hitscanWeaponData.ReloadDuration;
                audioSource.Play();
            }

            OnReloadStarted?.Invoke();

            yield return new WaitForSeconds(hitscanWeaponData.ReloadDuration);

            currentAmmo = hitscanWeaponData.MaxAmmo;

            if (audioSource != null)
                audioSource.pitch = 1f;

            bool shouldResumeFire = _fireInputHeld && currentAmmo > 0;

            _currentState = shouldResumeFire
                ? WeaponState.Firing
                : WeaponState.Idle;

            OnReloadFinished?.Invoke();

            if (shouldResumeFire)
            {
                _nextFireTime = Time.time;
                TryFire();
            }
        }

        private void StopFiringStateWithoutClearingInput()
        {
            if (_currentState != WeaponState.Firing)
                return;

            _currentState = WeaponState.Idle;
            OnFiredStoped?.Invoke();
        }
    }
}