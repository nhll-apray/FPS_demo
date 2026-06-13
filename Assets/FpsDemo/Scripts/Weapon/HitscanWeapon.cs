using System;
using System.Collections;
using FpsDemo.Combat;
using FpsDemo.Config;
using FpsDemo.Config.Weapon;
using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public class HitscanWeapon : WeaponBase
    {
        private HitscanWeaponConfig _hitscanWeaponConfig;
        private AudioClip _shootSound;
        private AudioClip _reloadSound;

        public HitscanWeaponConfig HitscanWeaponConfig => _hitscanWeaponConfig != null
            ? _hitscanWeaponConfig
            : _hitscanWeaponConfig = GameResources.LoadConfig<HitscanWeaponConfig>(GameResourcePaths.Config.Weapon.HitscanWeaponAk47);

        public HitscanWeaponConfig hitscanWeaponConfig => HitscanWeaponConfig;
        public override WeaponConfig WeaponConfig => HitscanWeaponConfig;

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
        private const float MinRecoilApplyDuration = 0.0001f;

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
            if (AimProvider == null)
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
                _nextFireTime = Time.time + hitscanWeaponConfig.FireInterval;
            }
            else
            {
                StopFiringStateWithoutClearingInput();
            }
        }

        private void Fire()
        {
            CurrentAmmo--;

            AudioClip shootSound = GetShootSound();
            if (AudioSource != null && shootSound != null)
            {
                AudioSource.PlayOneShot(shootSound);
            }
            
            OnFiredStarted?.Invoke();

            GameObject aimTarget = GetAimTarget();

            if (aimTarget != null)
            {
                IDamageable damageable = aimTarget.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    DamageSystem.ApplyDamage(damageable, new DamageInfo(hitscanWeaponConfig.Damage, Owner));
                }
            }
            
            float recoilScale = hitscanWeaponConfig.GetRecoilScale(_shotsFiredInBurst);
            _shotsFiredInBurst++;
            float pitchRecoil = hitscanWeaponConfig.RecoilPitch * recoilScale;
            float yawRecoil = hitscanWeaponConfig.RecoilYaw * recoilScale;

            CameraRecoilReceiver?.ApplyCameraRecoil(new CameraRecoilSettings
            {
                pitchPerShot = pitchRecoil,
                yawRandomRange = yawRecoil,
                applySpeed = GetRecoilApplySpeed(pitchRecoil, yawRecoil),
                recoverySpeed = hitscanWeaponConfig.RecoilRecoverySpeed,
                recoveryDelay = hitscanWeaponConfig.RecoilRecoveryDelay,
                maxQueuedPitch = hitscanWeaponConfig.MaxRecoilPitch,
                maxQueuedYaw = hitscanWeaponConfig.MaxRecoilYaw
            });
        }

        private AudioClip GetShootSound()
        {
            return _shootSound != null
                ? _shootSound
                : _shootSound = GameResources.LoadSfx(hitscanWeaponConfig.ShootSfxPath);
        }

        private AudioClip GetReloadSound()
        {
            return _reloadSound != null
                ? _reloadSound
                : _reloadSound = GameResources.LoadSfx(hitscanWeaponConfig.ReloadSfxPath);
        }

        private float GetRecoilApplySpeed(float pitchRecoil, float yawRecoil)
        {
            float maxSingleShotRecoil = Mathf.Max(Mathf.Abs(pitchRecoil), Mathf.Abs(yawRecoil));
            if (maxSingleShotRecoil <= 0f)
                return hitscanWeaponConfig.RecoilApplySpeed;

            float applyDuration = Mathf.Max(hitscanWeaponConfig.FireInterval, MinRecoilApplyDuration);
            float minApplySpeed = maxSingleShotRecoil / applyDuration;
            return Mathf.Max(hitscanWeaponConfig.RecoilApplySpeed, minApplySpeed);
        }

        public GameObject GetAimTarget()
        {
            if (Physics.Raycast(AimProvider.GetAimRay(), out RaycastHit hit, hitscanWeaponConfig.Range, LayerMask.GetMask("Enemy")))
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

            if (CurrentAmmo >= hitscanWeaponConfig.MaxAmmo)
                return;

            StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            StopFiringStateWithoutClearingInput();

            _currentState = WeaponState.Reloading;

            AudioClip reloadSound = GetReloadSound();
            if (AudioSource != null && reloadSound != null)
            {
                AudioSource.clip = reloadSound;
                AudioSource.pitch = reloadSound.length / hitscanWeaponConfig.ReloadDuration;
                AudioSource.Play();
            }

            OnReloadStarted?.Invoke();

            yield return new WaitForSeconds(hitscanWeaponConfig.ReloadDuration);

            CurrentAmmo = hitscanWeaponConfig.MaxAmmo;

            if (AudioSource != null)
                AudioSource.pitch = 1f;

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

            CameraRecoilReceiver?.BeginCameraRecoil();
        }

        private void StopFiringStateWithoutClearingInput()
        {
            if (_currentState != WeaponState.Firing)
                return;

            _currentState = WeaponState.Idle;
            CameraRecoilReceiver?.EndCameraRecoil();
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
