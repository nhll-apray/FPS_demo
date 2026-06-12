using System;
using UnityEngine;

namespace FpsDemo.Config.Player
{
    [Serializable]
    public struct LandingShakeSettings
    {
        public float duration;

        public float posDown;
        public float pitchDown;
        public float roll;

        public float shakeFrequency;
        public float shakePitch;
        public float shakeRoll;
    }

    [Serializable]
    public struct DamageShakeSettings
    {
        public float duration;
        public float positionKick;
        public float pitchKick;
        public float rollKick;
        public float rollNoise;
        public float shakeFrequency;
    }

    [CreateAssetMenu(
        fileName = "PlayerCameraEffectConfig",
        menuName = "Config/Player/Camera Effect")]
    public class PlayerCameraEffectConfig : ScriptableObject
    {
        public float minLandingSpeed = 2f;
        public float heavyLandingSpeed = 12f;

        [Header("小落地晃动")]
        public LandingShakeSettings smallLandingShake = new LandingShakeSettings
        {
            duration = 0.11f,
            posDown = 0.018f,
            pitchDown = 0.18f,
            roll = 0.04f,
            shakeFrequency = 10f,
            shakePitch = 0.04f,
            shakeRoll = 0.025f
        };

        [Header("大落地晃动")]
        public LandingShakeSettings heavyLandingShake = new LandingShakeSettings
        {
            duration = 0.20f,
            posDown = 0.12f,
            pitchDown = 1.6f,
            roll = 0.35f,
            shakeFrequency = 12f,
            shakePitch = 0.25f,
            shakeRoll = 0.16f
        };
    
        [Header("移动侧倾")]
        public float moveTiltAngle = 0.3f;
        public float moveTiltSpeed = 4f;

        [Header("Damage Shake")]
        public DamageShakeSettings damageShake = new DamageShakeSettings
        {
            duration = 0.16f,
            positionKick = 0.06f,
            pitchKick = 1.2f,
            rollKick = 0.8f,
            rollNoise = 0.25f,
            shakeFrequency = 16f
        };
    }
}
