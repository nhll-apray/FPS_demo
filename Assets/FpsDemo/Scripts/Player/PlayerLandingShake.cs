using UnityEngine;

namespace FpsDemo.Player
{
    public class PlayerLandingShake
    {
        private enum LandingType
        {
            None,
            Small,
            Heavy
        }

        public Vector3 PositionOffset { get; private set; }
        public Vector3 RotationOffset { get; private set; }

        private LandingType _currentLandingType = LandingType.None;

        private LandingShakeSettings _currentShakeSettings;

        private float _timer;
        private float _duration;
        private float _shakeSeed;

        public void Play(float fallSpeed, PlayerCameraEffectProfile profile)
        {
            if (profile == null)
                return;

            LandingType landingType = GetLandingType(fallSpeed, profile);

            if (landingType == LandingType.None)
                return;

            _currentLandingType = landingType;
            _currentShakeSettings = landingType == LandingType.Heavy ? profile.heavyLandingShake : profile.smallLandingShake;
            _duration = _currentShakeSettings.duration;
            _timer = _duration;
            _shakeSeed = Random.value * 100f;
        }

        public void Tick(float deltaTime)
        {
            PositionOffset = Vector3.zero;
            RotationOffset = Vector3.zero;

            if (_timer <= 0f || _duration <= 0f)
                return;

            //计算落地振动
            float elapsed = _duration - _timer;
            float t = Mathf.Clamp01(elapsed / _duration);
            float impactEnvelope = 1f - t;
            impactEnvelope *= impactEnvelope;
            float shakeEnvelope = 1f - t;
            float wave = Mathf.Sin(elapsed * _currentShakeSettings.shakeFrequency * Mathf.PI * 2f);
            float wave2 = Mathf.Sin((elapsed * _currentShakeSettings.shakeFrequency * 1.37f + _shakeSeed) * Mathf.PI * 2f);
            PositionOffset = new Vector3(0f, -_currentShakeSettings.posDown * impactEnvelope, 0f);
            float pitchOffset = _currentShakeSettings.pitchDown * impactEnvelope + wave * _currentShakeSettings.shakePitch * shakeEnvelope;
            float rollOffset = _currentShakeSettings.roll * impactEnvelope + wave2 * _currentShakeSettings.shakeRoll * shakeEnvelope;
            RotationOffset = new Vector3(pitchOffset, 0f, rollOffset);
        
            _timer -= deltaTime;
            if (_timer <= 0f)
            {
                Stop();
            }
        }

        public void Stop()
        {
            _timer = 0f;
            _duration = 0f;
            _currentLandingType = LandingType.None;

            PositionOffset = Vector3.zero;
            RotationOffset = Vector3.zero;
        }

        private LandingType GetLandingType(float fallSpeed, PlayerCameraEffectProfile profile)
        {
            fallSpeed = Mathf.Abs(fallSpeed);
            if (fallSpeed < profile.minLandingSpeed)
                return LandingType.None;
            if (fallSpeed >= profile.heavyLandingSpeed)
                return LandingType.Heavy;
            return LandingType.Small;
        }
    }
}