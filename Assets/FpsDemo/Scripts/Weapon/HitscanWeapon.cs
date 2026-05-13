using System.Collections;
using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public class HitscanWeapon : WeaponBase
    {
        [SerializeField] private HitscanWeaponData weaponData;
        public override WeaponData WeaponData => weaponData;

        private enum WeaponState
        {
            Idle,
            Firing,
            Reloading
        }

        private WeaponState _currentState;

        private float _nextFireTime;
        private Coroutine _stopAudioCoroutine;

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

            if (aimProvider == null)
                return;

            if (_currentState == WeaponState.Firing)
                return;

            _currentState = WeaponState.Firing;
            _nextFireTime = Time.time;

            CancelScheduledAudioStop();
            StartFiringAudio();

            TryFire();
        }

        public override void StopFire()
        {
            base.StopFire();

            if (_currentState != WeaponState.Firing)
                return;

            _currentState = WeaponState.Idle;

            ScheduleStopAudioAfterCurrentShot();
        }

        private void TryFire()
        {
            if (aimProvider == null)
                return;

            if (Time.time < _nextFireTime)
                return;

            if (currentAmmo <= 0)
            {
                _currentState = WeaponState.Idle;
                StopFiringAudioImmediately();
                return;
            }

            Fire();

            _nextFireTime = Time.time + weaponData.FireInterval;
        }

        private void Fire()
        {
            currentAmmo--;

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

        private void StartFiringAudio()
        {
            if (audioSource == null || weaponData.ShootSound == null)
                return;

            audioSource.clip = weaponData.ShootSound;
            audioSource.loop = true;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        private void ScheduleStopAudioAfterCurrentShot()
        {
            if (audioSource == null)
                return;

            CancelScheduledAudioStop();

            float delay = Mathf.Max(0f, _nextFireTime - Time.time);

            _stopAudioCoroutine = StartCoroutine(StopAudioAfterDelay(delay));
        }

        private IEnumerator StopAudioAfterDelay(float delay)
        {
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            _stopAudioCoroutine = null;

            // 如果等待期间玩家又重新按下开火，就不要停声音。
            if (_currentState == WeaponState.Firing)
                yield break;

            StopFiringAudioImmediately();
        }

        private void StopFiringAudioImmediately()
        {
            CancelScheduledAudioStop();

            if (audioSource == null)
                return;

            audioSource.loop = false;
            audioSource.Stop();
        }

        private void CancelScheduledAudioStop()
        {
            if (_stopAudioCoroutine == null)
                return;

            StopCoroutine(_stopAudioCoroutine);
            _stopAudioCoroutine = null;
        }

        public override void Reload()
        {
            base.Reload();

            _currentState = WeaponState.Reloading;
            StopFiringAudioImmediately();
        }
    }
}