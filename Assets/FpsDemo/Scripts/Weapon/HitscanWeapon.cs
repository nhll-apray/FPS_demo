using System;
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
        private HitscanWeaponRuntime _runtime;
        private HitscanHitResolver _hitResolver;
        private AudioClip _shootSound;
        private AudioClip _reloadSound;

        private const float MinRecoilApplyDuration = 0.0001f;

        public HitscanWeaponConfig HitscanWeaponConfig => _hitscanWeaponConfig != null
            ? _hitscanWeaponConfig
            : _hitscanWeaponConfig = GameResources.LoadConfig<HitscanWeaponConfig>(GameResourcePaths.Config.Weapon.HitscanWeaponAk47);

        public HitscanWeaponConfig hitscanWeaponConfig => HitscanWeaponConfig;
        public override WeaponConfig WeaponConfig => HitscanWeaponConfig;

        public bool IsReloading => _runtime != null && _runtime.IsReloading;
        public bool IsFiring => _runtime != null && _runtime.IsFiring;

        public event Action OnFiredStarted;
        public event Action OnFiredStoped;
        public event Action OnReloadStarted;
        public event Action OnReloadFinished;

        private void Update()
        {
            EnsureRuntime();
            _runtime.Tick(Time.time, IsAltFiring);
        }

        public override void StartFire()
        {
            EnsureRuntime();
            _runtime.StartFire(Time.time, IsAltFiring);
        }

        public override void StopFire()
        {
            EnsureRuntime();
            _runtime.StopFire();
        }

        public override void Reload()
        {
            EnsureRuntime();
            _runtime.Reload(Time.time, IsAltFiring);
        }

        public override void StartAltFire()
        {
            EnsureRuntime();

            if (_runtime.IsReloading)
                return;

            _runtime.InterruptFiringWithoutClearingInput();
            base.StartAltFire();
        }

        public GameObject GetAimTarget()
        {
            return TryGetDamageHit(out HitscanHitResult hitResult)
                ? hitResult.HitObject
                : null;
        }

        private void EnsureRuntime()
        {
            if (_runtime != null)
                return;

            _runtime = new HitscanWeaponRuntime(
                hitscanWeaponConfig.MaxAmmo,
                hitscanWeaponConfig.FireInterval,
                hitscanWeaponConfig.ReloadDuration,
                CanFire);

            _runtime.AmmoChanged += OnRuntimeAmmoChanged;
            _runtime.FiringStateEntered += OnRuntimeFiringStateEntered;
            _runtime.FiringStateExited += OnRuntimeFiringStateExited;
            _runtime.FireRequested += OnRuntimeFireRequested;
            _runtime.ReloadStarted += OnRuntimeReloadStarted;
            _runtime.ReloadFinished += OnRuntimeReloadFinished;
            EnsureHitResolver();

            if (CurrentAmmo != _runtime.CurrentAmmo)
            {
                CurrentAmmo = _runtime.CurrentAmmo;
            }
        }

        private void OnRuntimeAmmoChanged(int previousAmmo, int currentAmmo)
        {
            CurrentAmmo = currentAmmo;
        }

        private void EnsureHitResolver()
        {
            if (_hitResolver != null)
                return;

            _hitResolver = new HitscanHitResolver(LayerMask.GetMask("Enemy"));
        }

        private bool CanFire()
        {
            return AimProvider != null;
        }

        private void OnRuntimeFiringStateEntered()
        {
            CameraRecoilReceiver?.BeginCameraRecoil();
        }

        private void OnRuntimeFiringStateExited()
        {
            CameraRecoilReceiver?.EndCameraRecoil();
            OnFiredStoped?.Invoke();
        }

        private void OnRuntimeFireRequested(int shotIndex)
        {
            Fire(shotIndex);
        }

        private void OnRuntimeReloadStarted()
        {
            AudioClip reloadSound = GetReloadSound();
            if (AudioSource != null && reloadSound != null)
            {
                AudioSource.clip = reloadSound;
                AudioSource.pitch = reloadSound.length / hitscanWeaponConfig.ReloadDuration;
                AudioSource.Play();
            }

            OnReloadStarted?.Invoke();
        }

        private void OnRuntimeReloadFinished()
        {
            if (AudioSource != null)
                AudioSource.pitch = 1f;

            OnReloadFinished?.Invoke();
        }

        private void Fire(int shotIndex)
        {
            AudioClip shootSound = GetShootSound();
            if (AudioSource != null && shootSound != null)
            {
                AudioSource.PlayOneShot(shootSound);
            }

            OnFiredStarted?.Invoke();

            if (TryGetDamageHit(out HitscanHitResult hitResult))
            {
                int damage = GetDamageForHitZone(hitResult.HitZone);
                DamageSystem.ApplyDamage(
                    hitResult.Damageable,
                    new DamageInfo(damage, Owner, DamageType.Hitscan, hitResult.HitPoint, hitResult.HitZone));
            }

            float recoilScale = hitscanWeaponConfig.GetRecoilScale(shotIndex);
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

        private bool TryGetDamageHit(out HitscanHitResult hitResult)
        {
            hitResult = default;

            if (AimProvider == null)
                return false;

            EnsureHitResolver();
            return _hitResolver.TryResolve(
                AimProvider.GetAimRay(),
                hitscanWeaponConfig.Range,
                out hitResult);
        }

        private int GetDamageForHitZone(DamageHitZone hitZone)
        {
            if (hitZone == DamageHitZone.Head)
                return Mathf.Max(0, Mathf.RoundToInt(hitscanWeaponConfig.CritDamage));

            return hitscanWeaponConfig.Damage;
        }
    }
}
