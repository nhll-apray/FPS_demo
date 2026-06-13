using FpsDemo.Game;
using FpsDemo.Combat;
using FpsDemo.Config.Player;
using UnityEngine;

namespace FpsDemo.Player
{
    public class PlayerCameraEffects
    {
        private readonly PlayerLandingShake _landingShake = new PlayerLandingShake();
        private readonly PlayerCameraRecoil _recoil = new PlayerCameraRecoil();
        private readonly PlayerDamageShake _damageShake = new PlayerDamageShake();
    
        private PlayerCameraEffectConfig _config;
        private bool _isEnabled;

        public Vector2 AimRotationOffset => _recoil.AimRotationOffset;
        public Vector3 PositionOffset { get; private set; }
        public Vector3 RotationOffset { get; private set; }
    
        private float _currentMoveTilt;

        public PlayerCameraEffects(PlayerCameraEffectConfig config)
        {
            _config = config;
        }

        public void Enable()
        {
            if (_isEnabled)
                return;
        
            EventManager.AddListener<PlayerLandEvent>(OnPlayerLand);
            _isEnabled = true;
        }

        public void Disable()
        {
            if (!_isEnabled)
                return;
        
            EventManager.RemoveListener<PlayerLandEvent>(OnPlayerLand);
            _isEnabled = false;
        }

        public void TickBeforeLook(float deltaTime)
        {
            _recoil.TickBeforeLook(deltaTime);
        }

        public void TickAfterLook(float deltaTime)
        {
            _recoil.TickAfterLook(deltaTime);
        }

        public void Tick(float deltaTime, Vector2 moveInput)
        {
            UpdateMoveTilt(deltaTime, moveInput);
            _landingShake.Tick(deltaTime);
            _damageShake.Tick(deltaTime);
            PositionOffset = _landingShake.PositionOffset + _damageShake.PositionOffset;
            RotationOffset = new Vector3(0f, 0f, _currentMoveTilt) + _landingShake.RotationOffset + _damageShake.RotationOffset;
        }

        public Vector2 CommitAimRecoil()
        {
            return _recoil.CommitAsAim();
        }

        public void BeginCameraRecoil()
        {
            _recoil.Begin();
        }

        public void ApplyCameraRecoil(CameraRecoilSettings recoil)
        {
            _recoil.Apply(recoil);
        }

        public void EndCameraRecoil()
        {
            _recoil.End();
        }

        public void PlayDamageShake(float intensity)
        {
            _damageShake.Play(intensity, _config);
        }

        public void StopAll()
        {
            _landingShake.Stop();
            _recoil.Stop();
            _damageShake.Stop();

            PositionOffset = Vector3.zero;
            RotationOffset = Vector3.zero;
        }
    
        //�ƶ�����
        private void UpdateMoveTilt(float deltaTime, Vector2 moveInput)
        {
            if (_config == null)
            {
                _currentMoveTilt = 0f;
                return;
            }

            float targetTilt = -moveInput.x * _config.moveTiltAngle;
            _currentMoveTilt = Mathf.Lerp(_currentMoveTilt, targetTilt, _config.moveTiltSpeed * deltaTime);
        }

        private void OnPlayerLand(PlayerLandEvent evt)
        {
            _landingShake.Play(evt.Velocity, _config);
        }
    }
}
