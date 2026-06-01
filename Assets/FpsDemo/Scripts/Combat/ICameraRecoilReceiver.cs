using System;
using UnityEngine;

namespace FpsDemo.Combat
{
    [Serializable]
    public struct CameraRecoilSettings
    {
        public float pitchPerShot;
        public float yawRandomRange;
        public float applySpeed;
        public float recoverySpeed;
        public float recoveryDelay;
        public float maxQueuedPitch;
        public float maxQueuedYaw;
    }

    public interface ICameraRecoilReceiver
    {
        void BeginCameraRecoil();
        void ApplyCameraRecoil(CameraRecoilSettings recoil);
        void EndCameraRecoil();
    }
}
