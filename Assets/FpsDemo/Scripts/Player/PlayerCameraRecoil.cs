using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Player
{
    public class PlayerCameraRecoil
    {
        private const float DefaultApplySpeed = 18f;
        private const float DefaultRecoverySpeed = 8f;
        private const float DefaultRecoveryDelay = 0.08f;

        private float _queuedPitch;
        private float _queuedYaw;
        private float _applySpeed = DefaultApplySpeed;
        private float _recoverySpeed = DefaultRecoverySpeed;
        private float _recoveryDelay = DefaultRecoveryDelay;
        private float _recoilEndedTime = float.NegativeInfinity;
        private bool _isApplying;
        private int _yawDirection = 1;
        private int _shotsBeforeYawDirectionChange;
        private Vector2 _aimRotationOffset;

        public Vector2 AimRotationOffset => _aimRotationOffset;

        public void Begin()
        {
            _isApplying = true;
        }

        public void Apply(CameraRecoilSettings recoil)
        {
            if (recoil.applySpeed > 0f)
            {
                _applySpeed = recoil.applySpeed;
            }
            if (recoil.recoverySpeed > 0f)
            {
                _recoverySpeed = recoil.recoverySpeed;
            }
            if (recoil.recoveryDelay >= 0f)
            {
                _recoveryDelay = recoil.recoveryDelay;
            }

            _queuedPitch += Mathf.Max(0f, recoil.pitchPerShot);
            if (recoil.maxQueuedPitch > 0f)
            {
                _queuedPitch = Mathf.Min(_queuedPitch, recoil.maxQueuedPitch);
            }

            _queuedYaw += GetHorizontalRecoilStep(recoil.yawRandomRange);
            if (recoil.maxQueuedYaw > 0f)
            {
                _queuedYaw = Mathf.Clamp(_queuedYaw, -recoil.maxQueuedYaw, recoil.maxQueuedYaw);
            }
        }

        public void End()
        {
            if (!_isApplying)
                return;

            _isApplying = false;
            _recoilEndedTime = Time.time;
        }

        public void TickBeforeLook(float deltaTime)
        {
            ApplyQueuedRecoil(deltaTime);
        }

        public void TickAfterLook(float deltaTime)
        {
            RecoverAppliedRecoil(deltaTime);
        }

        public Vector2 CommitAsAim()
        {
            Vector2 committedOffset = _aimRotationOffset;
            _aimRotationOffset = Vector2.zero;
            return committedOffset;
        }

        public void Stop()
        {
            _queuedPitch = 0f;
            _queuedYaw = 0f;
            _aimRotationOffset = Vector2.zero;
            _isApplying = false;
            _recoilEndedTime = float.NegativeInfinity;
        }

        private void ApplyQueuedRecoil(float deltaTime)
        {
            if (_queuedPitch <= 0f && Mathf.Approximately(_queuedYaw, 0f))
                return;

            float maxStep = _applySpeed * deltaTime;

            float pitchStep = Mathf.Min(_queuedPitch, maxStep);
            _queuedPitch -= pitchStep;
            _aimRotationOffset.x -= pitchStep;

            float yawStep = Mathf.MoveTowards(0f, _queuedYaw, maxStep);
            _queuedYaw -= yawStep;
            _aimRotationOffset.y += yawStep;
        }

        private void RecoverAppliedRecoil(float deltaTime)
        {
            if (_isApplying)
                return;

            if (Time.time - _recoilEndedTime < _recoveryDelay)
                return;

            if (_queuedPitch > 0f || !Mathf.Approximately(_queuedYaw, 0f))
                return;

            if (Mathf.Approximately(_aimRotationOffset.x, 0f) && Mathf.Approximately(_aimRotationOffset.y, 0f))
                return;

            float maxStep = _recoverySpeed * deltaTime;
            _aimRotationOffset = Vector2.MoveTowards(_aimRotationOffset, Vector2.zero, maxStep);
        }

        private float GetHorizontalRecoilStep(float yawRange)
        {
            if (yawRange <= 0f)
                return 0f;

            if (_shotsBeforeYawDirectionChange <= 0 || Random.value < 0.18f)
            {
                _yawDirection = Random.value < 0.5f ? -1 : 1;
                _shotsBeforeYawDirectionChange = Random.Range(2, 5);
            }

            _shotsBeforeYawDirectionChange--;

            float magnitude = Random.Range(yawRange * 0.65f, yawRange);
            return magnitude * _yawDirection;
        }
    }
}
