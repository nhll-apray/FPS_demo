using FpsDemo.Player;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public sealed class WeaponAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private HitscanWeapon hitscanWeapon;
        [SerializeField] private GrenadeAltFire grenadeAltFire;
        [SerializeField] private AnimationClip reloadClip;
        [SerializeField] private AnimationClip fireClip;

        private WeaponAnimationPresenter _presenter;
        private PlayerMovement _playerMovement;

        private void Awake()
        {
            ResolveReferences();
            _presenter = new WeaponAnimationPresenter(animator);
            ApplyAnimationSpeeds();
        }

        private void OnEnable()
        {
            ResolveReferences();
            SubscribeWeaponEvents();
            ApplyAnimationSpeeds();
        }

        private void OnDisable()
        {
            UnsubscribeWeaponEvents();
        }

        private void Update()
        {
            ResolvePlayerMovement();
            UpdateMoveState();
        }

        private void ResolveReferences()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            if (hitscanWeapon == null)
                hitscanWeapon = GetComponent<HitscanWeapon>();

            if (grenadeAltFire == null)
                grenadeAltFire = GetComponent<GrenadeAltFire>();
        }

        private void SubscribeWeaponEvents()
        {
            if (hitscanWeapon != null)
            {
                hitscanWeapon.OnFiredStarted += OnFiredStarted;
                hitscanWeapon.OnFiredStoped += OnFiredStoped;
                hitscanWeapon.OnReloadStarted += OnReloadStarted;
                hitscanWeapon.OnReloadFinished += OnReloadFinished;
            }

            if (grenadeAltFire != null)
            {
                grenadeAltFire.OnStarted += OnAltFireStarted;
                grenadeAltFire.OnFinished += OnAltFireFinished;
            }
        }

        private void UnsubscribeWeaponEvents()
        {
            if (hitscanWeapon != null)
            {
                hitscanWeapon.OnFiredStarted -= OnFiredStarted;
                hitscanWeapon.OnFiredStoped -= OnFiredStoped;
                hitscanWeapon.OnReloadStarted -= OnReloadStarted;
                hitscanWeapon.OnReloadFinished -= OnReloadFinished;
            }

            if (grenadeAltFire != null)
            {
                grenadeAltFire.OnStarted -= OnAltFireStarted;
                grenadeAltFire.OnFinished -= OnAltFireFinished;
            }
        }

        private void ApplyAnimationSpeeds()
        {
            if (_presenter == null || hitscanWeapon == null)
                return;

            float reloadSpeed = CalculateClipSpeed(reloadClip, hitscanWeapon.hitscanWeaponConfig.ReloadDuration);
            float fireSpeed = CalculateClipSpeed(fireClip, hitscanWeapon.hitscanWeaponConfig.FireInterval);
            _presenter.SetReloadSpeed(reloadSpeed);
            _presenter.SetFireSpeed(fireSpeed);
        }

        private static float CalculateClipSpeed(AnimationClip clip, float targetDuration)
        {
            if (clip == null || targetDuration <= 0f)
                return 1f;

            return clip.length / targetDuration;
        }

        private void ResolvePlayerMovement()
        {
            if (_playerMovement != null || hitscanWeapon == null || hitscanWeapon.Owner == null)
                return;

            _playerMovement = hitscanWeapon.Owner.GetComponent<PlayerMovement>();
        }

        private void UpdateMoveState()
        {
            if (_playerMovement == null)
                return;

            if (_playerMovement.IsSprinting)
            {
                _presenter?.SetMoveState(WeaponMoveState.Run);
                return;
            }

            if (_playerMovement.IsWalking)
            {
                _presenter?.SetMoveState(WeaponMoveState.Walk);
                return;
            }

            _presenter?.SetMoveState(WeaponMoveState.Idle);
        }

        private void OnFiredStarted()
        {
            _presenter?.PlayFire();
        }

        private void OnFiredStoped()
        {
            _presenter?.StopFire();
        }

        private void OnReloadStarted()
        {
            _presenter?.StartReload();
        }

        private void OnReloadFinished()
        {
            _presenter?.FinishReload();
        }

        private void OnAltFireStarted()
        {
            _presenter?.StartAltFire();
        }

        private void OnAltFireFinished()
        {
            _presenter?.FinishAltFire();
        }
    }
}
