using UnityEngine;

namespace FpsDemo.Weapon
{
    public enum WeaponMoveState
    {
        Idle = 0,
        Walk = 1,
        Run = 2
    }

    public sealed class WeaponAnimationPresenter
    {
        private static readonly int TriggerFire = Animator.StringToHash("Fire");
        private static readonly int TriggerReload = Animator.StringToHash("Reload");
        private static readonly int TriggerThrowGrenade = Animator.StringToHash("ThrowGrenade");
        private static readonly int BoolIsFiring = Animator.StringToHash("IsFiring");
        private static readonly int BoolIsReloading = Animator.StringToHash("IsReloading");
        private static readonly int BoolIsAltFiring = Animator.StringToHash("IsAltFiring");
        private static readonly int IntMoveState = Animator.StringToHash("MoveState");
        private static readonly int FloatReloadSpeed = Animator.StringToHash("ReloadSpeed");
        private static readonly int FloatFireSpeed = Animator.StringToHash("FireSpeed");

        private readonly Animator _animator;

        public WeaponAnimationPresenter(Animator animator)
        {
            _animator = animator;
        }

        public void SetReloadSpeed(float speed)
        {
            _animator?.SetFloat(FloatReloadSpeed, speed);
        }

        public void SetFireSpeed(float speed)
        {
            _animator?.SetFloat(FloatFireSpeed, speed);
        }

        public void PlayFire()
        {
            if (_animator == null)
                return;

            _animator.SetTrigger(TriggerFire);
            _animator.SetBool(BoolIsFiring, true);
        }

        public void StopFire()
        {
            _animator?.SetBool(BoolIsFiring, false);
        }

        public void StartReload()
        {
            if (_animator == null)
                return;

            _animator.SetTrigger(TriggerReload);
            _animator.SetBool(BoolIsReloading, true);
        }

        public void FinishReload()
        {
            _animator?.SetBool(BoolIsReloading, false);
        }

        public void StartAltFire()
        {
            if (_animator == null)
                return;

            _animator.SetTrigger(TriggerThrowGrenade);
            _animator.SetBool(BoolIsAltFiring, true);
        }

        public void FinishAltFire()
        {
            _animator?.SetBool(BoolIsAltFiring, false);
        }

        public void SetMoveState(WeaponMoveState moveState)
        {
            _animator?.SetInteger(IntMoveState, (int)moveState);
        }
    }
}
