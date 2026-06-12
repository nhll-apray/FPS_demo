using FpsDemo.Config.Player;
using UnityEngine;

namespace FpsDemo.Player
{
    public class PlayerDamageShake
    {
        public Vector3 PositionOffset { get; private set; }
        public Vector3 RotationOffset { get; private set; }

        private DamageShakeSettings _settings;
        private float _timer;
        private float _duration;
        private float _intensity;
        private float _rollDirection;
        private float _seed;

        public void Play(float intensity, PlayerCameraEffectConfig config)
        {
            if (config == null)
            {
                return;
            }

            _settings = config.damageShake;
            _duration = Mathf.Max(0f, _settings.duration);
            if (_duration <= 0f)
            {
                Stop();
                return;
            }

            _timer = _duration;
            _intensity = Mathf.Clamp01(intensity);
            _rollDirection = Random.value < 0.5f ? -1f : 1f;
            _seed = Random.value * 100f;
        }

        public void Tick(float deltaTime)
        {
            PositionOffset = Vector3.zero;
            RotationOffset = Vector3.zero;

            if (_timer <= 0f || _duration <= 0f)
            {
                return;
            }

            float elapsed = _duration - _timer;
            float t = Mathf.Clamp01(elapsed / _duration);
            float envelope = 1f - t;
            envelope *= envelope;
            float wave = Mathf.Sin((elapsed * _settings.shakeFrequency + _seed) * Mathf.PI * 2f);

            float strength = _intensity * envelope;
            PositionOffset = new Vector3(0f, 0f, -_settings.positionKick * strength);
            RotationOffset = new Vector3(-_settings.pitchKick * strength, 0f, (_settings.rollKick * _rollDirection + wave * _settings.rollNoise) * strength);

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
            _intensity = 0f;
            PositionOffset = Vector3.zero;
            RotationOffset = Vector3.zero;
        }
    }
}
