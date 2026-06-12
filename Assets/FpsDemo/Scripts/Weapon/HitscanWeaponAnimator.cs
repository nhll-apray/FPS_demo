using System;
using FpsDemo.Player;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public class HitscanWeaponAnimator : MonoBehaviour
    {
        private Animator _animator;
        private HitscanWeapon _hitscanWeapon;
        private PlayerMovement _playerMovement;
        
        private static readonly int TriggerFire = Animator.StringToHash("Fire");
        private static readonly int TriggerReload = Animator.StringToHash("Reload");
        private static readonly int BoolIsFiring = Animator.StringToHash("IsFiring");
        private static readonly int BoolIsReloading = Animator.StringToHash("IsReloading");
        private static readonly int IntMoveState = Animator.StringToHash("MoveState");
        private static readonly int FloatReloadSpeed = Animator.StringToHash("ReloadSpeed");
        private static readonly int FloatFireSpeed = Animator.StringToHash("FireSpeed");

        private const int IdleMoveState = 0;
        private const int WalkMoveState = 1;
        private const int RunMoveState = 2;
        
        [SerializeField] private AnimationClip reloadClip;
        [SerializeField] private AnimationClip fireClip;
        

        private float _reloadSpeed;
        private float _fireSpeed;
        
        

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _hitscanWeapon = GetComponent<HitscanWeapon>();
            _reloadSpeed = reloadClip.length / _hitscanWeapon.hitscanWeaponConfig.ReloadDuration;
            _fireSpeed = fireClip.length / _hitscanWeapon.hitscanWeaponConfig.FireInterval;
        }

        private void OnEnable()
        {
            _hitscanWeapon.OnFiredStarted += OnFiredStarted;
            _hitscanWeapon.OnFiredStoped += OnFiredStoped;
            _hitscanWeapon.OnReloadStarted += OnReloadStarted;
            _hitscanWeapon.OnReloadFinished += OnReloadFinished;
            _animator.SetFloat(FloatReloadSpeed, _reloadSpeed);
            _animator.SetFloat(FloatFireSpeed, _fireSpeed);
        }

        private void OnDisable()
        {
            _hitscanWeapon.OnFiredStarted -= OnFiredStarted;
            _hitscanWeapon.OnFiredStoped -= OnFiredStoped;
            _hitscanWeapon.OnReloadStarted -= OnReloadStarted;
            _hitscanWeapon.OnReloadFinished -= OnReloadFinished;
        }

        private void Update()
        {
            if (_hitscanWeapon.Owner != null && _playerMovement == null)
            {
                _playerMovement = _hitscanWeapon.Owner.GetComponent<PlayerMovement>();
            }
            if (_playerMovement != null)
            {
                int moveState = IdleMoveState;
                if (_playerMovement.IsSprinting)
                    moveState = RunMoveState;
                else if (_playerMovement.IsWalking)
                    moveState = WalkMoveState;

                _animator.SetInteger(IntMoveState, moveState);
            }
        }

        private void OnFiredStarted()
        {
            _animator.SetTrigger(TriggerFire);
            _animator.SetBool(BoolIsFiring, true);
        }

        private void OnFiredStoped()
        {
            _animator.SetBool(BoolIsFiring, false);
        }
        
        private void OnReloadStarted()
        {
            _animator.SetTrigger(TriggerReload);
            _animator.SetBool(BoolIsReloading, true);
        }

        private void OnReloadFinished()
        {
            _animator.SetBool(BoolIsReloading, false);
        }
    }
}
