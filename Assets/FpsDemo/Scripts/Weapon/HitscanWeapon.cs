using System;
using System.Collections;
using FpsDemo.Combat;
using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public class HitscanWeapon : WeaponBase
    {
        private HitscanWeaponData _hitscanWeaponData;

        public HitscanWeaponData HitscanWeaponData => _hitscanWeaponData != null
            ? _hitscanWeaponData
            : _hitscanWeaponData = GameResources.LoadData<HitscanWeaponData>(GameResourcePaths.Data.Weapon.HitscanWeaponDataAK47);

        public HitscanWeaponData hitscanWeaponData => HitscanWeaponData;
        public override WeaponData WeaponData => HitscanWeaponData;

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
        
        private bool _fireInputHeld;
        private int _shotsFiredInBurst;

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

            if (IsAltFiring)
                return;
            
            if (IsReloading)
                return;

            if (_currentState == WeaponState.Firing)
                return;

            base.StartFire();

            EnterFiringState(resetBurst: true);
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

            if (CurrentAmmo > 0)
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
            CurrentAmmo--;

            if (audioSource != null && hitscanWeaponData.ShootSound != null)
            {
                audioSource.PlayOneShot(hitscanWeaponData.ShootSound);
            }
            
            OnFiredStarted?.Invoke();

            GameObject aimTarget = GetAimTarget();

            if (aimTarget != null)
            {
                IDamageable damageable = aimTarget.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    DamageSystem.ApplyDamage(damageable, new DamageInfo(hitscanWeaponData.Damage, Owner));
                }
            }
            
            float recoilScale = hitscanWeaponData.GetRecoilScale(_shotsFiredInBurst);
            _shotsFiredInBurst++;

            cameraRecoilReceiver?.ApplyCameraRecoil(new CameraRecoilSettings
            {
                pitchPerShot = hitscanWeaponData.RecoilPitch * recoilScale,
                yawRandomRange = hitscanWeaponData.RecoilYaw * recoilScale,
                applySpeed = hitscanWeaponData.RecoilApplySpeed,
                recoverySpeed = hitscanWeaponData.RecoilRecoverySpeed,
                recoveryDelay = hitscanWeaponData.RecoilRecoveryDelay,
                maxQueuedPitch = hitscanWeaponData.MaxRecoilPitch,
                maxQueuedYaw = hitscanWeaponData.MaxRecoilYaw
            });
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
            if (IsAltFiring)
                return;
            
            if (_currentState == WeaponState.Reloading)
                return;

            if (CurrentAmmo >= hitscanWeaponData.MaxAmmo)
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

            CurrentAmmo = hitscanWeaponData.MaxAmmo;

            if (audioSource != null)
                audioSource.pitch = 1f;

            bool shouldResumeFire = _fireInputHeld && CurrentAmmo > 0;

            if (shouldResumeFire)
            {
                EnterFiringState(resetBurst: true);
                _nextFireTime = Time.time;
            }
            else
            {
                _currentState = WeaponState.Idle;
            }

            OnReloadFinished?.Invoke();

            if (shouldResumeFire)
            {
                TryFire();
            }
        }

        private void EnterFiringState(bool resetBurst)
        {
            _currentState = WeaponState.Firing;

            if (resetBurst)
            {
                _shotsFiredInBurst = 0;
            }

            cameraRecoilReceiver?.BeginCameraRecoil();
        }

        private void StopFiringStateWithoutClearingInput()
        {
            if (_currentState != WeaponState.Firing)
                return;

            _currentState = WeaponState.Idle;
            cameraRecoilReceiver?.EndCameraRecoil();
            OnFiredStoped?.Invoke();
        }
        
        public override void StartAltFire()
        {
            if (IsReloading)
                return;

            StopFiringStateWithoutClearingInput();
            base.StartAltFire();
        }
    }
}
