using System;

namespace FpsDemo.Weapon
{
    public sealed class GrenadeAltFireRuntime
    {
        private enum AltFireState
        {
            Idle,
            Windup,
            Finishing
        }

        private AltFireState _state;
        private GrenadeAltFireRuntimeSettings _settings;
        private float _releaseTime;
        private float _finishTime;
        private float _nextReadyTime;
        private float _cooldownDisplayDuration;
        private bool _hasReleased;

        public bool IsActive => _state != AltFireState.Idle;

        public event Action ReleaseRequested;
        public event Action Finished;

        public bool CanStart(float time)
        {
            return !IsActive && time >= _nextReadyTime;
        }

        public bool TryStart(float time, GrenadeAltFireRuntimeSettings settings)
        {
            if (!CanStart(time))
                return false;

            _settings = settings;
            _state = AltFireState.Windup;
            _hasReleased = false;
            _releaseTime = time + settings.ReleaseDelay;
            float finishDelay = Math.Max(settings.ReleaseDelay, settings.FinishDelay);
            _finishTime = time + finishDelay;
            _cooldownDisplayDuration = finishDelay + settings.Cooldown;
            _nextReadyTime = time + _cooldownDisplayDuration;
            return true;
        }

        public float GetCooldownRemaining(float time)
        {
            return Math.Max(0f, _nextReadyTime - time);
        }

        public float GetCooldownNormalized(float time)
        {
            if (_cooldownDisplayDuration <= 0f)
                return 0f;

            return Math.Min(1f, GetCooldownRemaining(time) / _cooldownDisplayDuration);
        }

        public bool IsCoolingDown(float time)
        {
            return GetCooldownRemaining(time) > 0f;
        }

        public void Tick(float time)
        {
            if (_state == AltFireState.Idle)
                return;

            if (!_hasReleased && time >= _releaseTime)
            {
                _hasReleased = true;
                _state = AltFireState.Finishing;
                ReleaseRequested?.Invoke();
            }

            if (_state != AltFireState.Idle && time >= _finishTime)
            {
                Complete(time);
            }
        }

        private void Complete(float time)
        {
            _state = AltFireState.Idle;
            _nextReadyTime = time + _settings.Cooldown;
            Finished?.Invoke();
        }
    }

    public readonly struct GrenadeAltFireRuntimeSettings
    {
        public readonly float ReleaseDelay;
        public readonly float FinishDelay;
        public readonly float Cooldown;

        public GrenadeAltFireRuntimeSettings(float releaseDelay, float finishDelay, float cooldown)
        {
            ReleaseDelay = Math.Max(0f, releaseDelay);
            FinishDelay = Math.Max(0f, finishDelay);
            Cooldown = Math.Max(0f, cooldown);
        }
    }
}
