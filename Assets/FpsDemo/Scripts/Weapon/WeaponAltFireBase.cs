using System;
using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public abstract class WeaponAltFireBase : MonoBehaviour
    {
        public bool IsActive { get; protected set; }

        public event Action OnStarted;
        public event Action OnReleased;
        public event Action OnFinished;

        public bool TryStart(WeaponBase weapon, WeaponUseContext context)
        {
            if (IsActive)
                return false;

            if (!CanStart(weapon, context))
                return false;

            IsActive = true;
            OnStarted?.Invoke();
            OnStart(weapon, context);
            return true;
        }

        public virtual void Stop(WeaponBase weapon, WeaponUseContext context) {}

        protected virtual bool CanStart(WeaponBase weapon, WeaponUseContext context) => true;

        protected abstract void OnStart(WeaponBase weapon, WeaponUseContext context);

        protected void NotifyReleased()
        {
            OnReleased?.Invoke();
        }

        protected void Finish()
        {
            IsActive = false;
            OnFinished?.Invoke();
        }
    }
}
