using FpsDemo.Game;
using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Player
{
    public class PlayerCameraEffects
    {
        //落地振动
        private readonly PlayerLandingShake _landingShake = new PlayerLandingShake();
        private readonly PlayerCameraRecoil _recoil = new PlayerCameraRecoil();
    
        private PlayerCameraEffectProfile _profile;
        private bool _isEnabled;

        public Vector2 AimRotationOffset => _recoil.AimRotationOffset;
        public Vector3 PositionOffset { get; private set; }
        public Vector3 RotationOffset { get; private set; }
    
        private float _currentMoveTilt;

        public PlayerCameraEffects(PlayerCameraEffectProfile profile)
        {
            _profile = profile;
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
            PositionOffset = _landingShake.PositionOffset;
            RotationOffset = new Vector3(0f, 0f, _currentMoveTilt) + _landingShake.RotationOffset;
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

        public void StopAll()
        {
            _landingShake.Stop();
            _recoil.Stop();

            PositionOffset = Vector3.zero;
            RotationOffset = Vector3.zero;
        }
    
        //移动侧倾
        private void UpdateMoveTilt(float deltaTime, Vector2 moveInput)
        {
            if (_profile == null)
            {
                _currentMoveTilt = 0f;
                return;
            }

            float targetTilt = -moveInput.x * _profile.moveTiltAngle;
            _currentMoveTilt = Mathf.Lerp(_currentMoveTilt, targetTilt, _profile.moveTiltSpeed * deltaTime);
        }

        private void OnPlayerLand(PlayerLandEvent evt)
        {
            _landingShake.Play(evt.velocity, _profile);
        }
    }
}
