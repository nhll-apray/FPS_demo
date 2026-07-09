using System;

namespace FpsDemo.Weapon
{
    public sealed class HitscanWeaponRuntime
    {
        private enum WeaponState
        {
            Idle,
            Firing,
            Reloading
        }

        private readonly int _maxAmmo;
        private readonly float _fireInterval;
        private readonly float _reloadDuration;
        private readonly Func<bool> _canFire;

        private WeaponState _currentState;
        private float _nextFireTime;
        private bool _autoReloadQueued;
        private float _autoReloadTime;
        private bool _fireInputHeld;
        private int _shotsFiredInBurst;
        private float _reloadFinishTime;

        public HitscanWeaponRuntime(int maxAmmo, float fireInterval, float reloadDuration, Func<bool> canFire)
        {
            _maxAmmo = Math.Max(0, maxAmmo);
            _fireInterval = Math.Max(0f, fireInterval);
            _reloadDuration = Math.Max(0f, reloadDuration);
            _canFire = canFire;
            CurrentAmmo = _maxAmmo;
        }

        public int CurrentAmmo { get; private set; }
        public bool IsReloading => _currentState == WeaponState.Reloading;
        public bool IsFiring => _currentState == WeaponState.Firing;

        public event Action<int, int> AmmoChanged;
        public event Action FiringStateEntered;
        public event Action FiringStateExited;
        public event Action<int> FireRequested;
        public event Action ReloadStarted;
        public event Action ReloadFinished;

        public void Tick(float time, bool isAltFiring)
        {
            TryProcessAutoReload(time, isAltFiring);
            TryCompleteReload(time);

            if (_currentState == WeaponState.Firing)
            {
                TryFire(time);
            }
        }

        public void StartFire(float time, bool isAltFiring)
        {
            _fireInputHeld = true;

            if (isAltFiring || IsReloading || _currentState == WeaponState.Firing)
                return;

            EnterFiringState(resetBurst: true);
            _nextFireTime = time;
            TryFire(time);
        }

        public void StopFire()
        {
            _fireInputHeld = false;

            if (IsReloading)
                return;

            StopFiringStateWithoutClearingInput();
        }

        public void Reload(float time, bool isAltFiring)
        {
            if (isAltFiring)
                return;

            if (_currentState == WeaponState.Reloading)
                return;

            if (CurrentAmmo >= _maxAmmo)
                return;

            _autoReloadQueued = false;
            StopFiringStateWithoutClearingInput();

            _currentState = WeaponState.Reloading;
            _reloadFinishTime = time + _reloadDuration;
            ReloadStarted?.Invoke();
        }

        public void InterruptFiringWithoutClearingInput()
        {
            StopFiringStateWithoutClearingInput();
        }

        private void TryFire(float time)
        {
            if (_currentState != WeaponState.Firing)
                return;

            if (_canFire != null && !_canFire())
                return;

            if (time < _nextFireTime)
                return;

            if (CurrentAmmo > 0)
            {
                SetAmmo(CurrentAmmo - 1);

                int shotIndex = _shotsFiredInBurst;
                FireRequested?.Invoke(shotIndex);
                _shotsFiredInBurst++;

                _nextFireTime = time + _fireInterval;

                if (CurrentAmmo <= 0)
                {
                    QueueAutoReload(_fireInterval, time);
                }

                return;
            }

            StopFiringStateWithoutClearingInput();
            QueueAutoReload(0f, time);
        }

        private void QueueAutoReload(float delay, float time)
        {
            if (_autoReloadQueued || IsReloading || CurrentAmmo > 0)
                return;

            _autoReloadQueued = true;
            _autoReloadTime = time + Math.Max(0f, delay);
        }

        private void TryProcessAutoReload(float time, bool isAltFiring)
        {
            if (!_autoReloadQueued || time < _autoReloadTime)
                return;

            if (isAltFiring)
                return;

            _autoReloadQueued = false;

            if (CurrentAmmo <= 0 && !IsReloading)
            {
                Reload(time, isAltFiring: false);
            }
        }

        private void TryCompleteReload(float time)
        {
            if (_currentState != WeaponState.Reloading || time < _reloadFinishTime)
                return;

            SetAmmo(_maxAmmo);

            bool shouldResumeFire = _fireInputHeld && CurrentAmmo > 0;

            if (shouldResumeFire)
            {
                EnterFiringState(resetBurst: true);
                _nextFireTime = time;
            }
            else
            {
                _currentState = WeaponState.Idle;
            }

            ReloadFinished?.Invoke();

            if (shouldResumeFire)
            {
                TryFire(time);
            }
        }

        private void EnterFiringState(bool resetBurst)
        {
            _currentState = WeaponState.Firing;

            if (resetBurst)
            {
                _shotsFiredInBurst = 0;
            }

            FiringStateEntered?.Invoke();
        }

        private void StopFiringStateWithoutClearingInput()
        {
            if (_currentState != WeaponState.Firing)
                return;

            _currentState = WeaponState.Idle;
            FiringStateExited?.Invoke();
        }

        private void SetAmmo(int value)
        {
            int previousAmmo = CurrentAmmo;
            CurrentAmmo = Math.Min(Math.Max(value, 0), _maxAmmo);
            AmmoChanged?.Invoke(previousAmmo, CurrentAmmo);
        }
    }
}
