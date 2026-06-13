using UnityEngine;
using UnityEngine.Serialization;

namespace FpsDemo.Config.Weapon
{
    [CreateAssetMenu(fileName = "HitscanWeaponConfig", menuName = "Config/Weapon/Hitscan Weapon", order = 0)]
    public class HitscanWeaponConfig : WeaponConfig
    {
        [SerializeField] private int damage = 10;
        [SerializeField] private float critDamage = 50f;
        [SerializeField] private float reloadDuration = 2.5f;
        [SerializeField] private float range = 100f;
        [SerializeField] private float fireInterval = 0.4f;
        
        [SerializeField] private float recoilPitch = 0.35f;
        [SerializeField] private float recoilYaw = 0.08f;
        [SerializeField] private float recoilApplySpeed = 18f;
        [SerializeField] private float recoilRecoverySpeed = 8f;
        [SerializeField] private float recoilRecoveryDelay = 0.08f;
        [SerializeField] private float maxRecoilPitch = 3f;
        [SerializeField] private float maxRecoilYaw = 1f;
        [SerializeField] private int accurateRecoilShots = 3;
        [Range(0f, 1f)]
        [SerializeField] private float accurateRecoilMultiplier = 0.2f;
        [SerializeField] private int recoilRampShots = 4;
        [SerializeField] private string shootSfxPath = GameResourcePaths.Audio.Sfx.Weapon.AK47.Shoot;
        [SerializeField] private string reloadSfxPath = GameResourcePaths.Audio.Sfx.Weapon.AK47.Reload;
        
        public int Damage => damage;
        public float CritDamage => critDamage;
        public float ReloadDuration => reloadDuration;
        public float Range => range;
        public float FireInterval => fireInterval;
        public string ShootSfxPath => shootSfxPath;
        public string ReloadSfxPath => reloadSfxPath;
        
        public float RecoilPitch => recoilPitch;
        public float RecoilYaw => recoilYaw;
        public float RecoilApplySpeed => recoilApplySpeed;
        public float RecoilRecoverySpeed => recoilRecoverySpeed;
        public float RecoilRecoveryDelay => recoilRecoveryDelay;
        public float MaxRecoilPitch => maxRecoilPitch;
        public float MaxRecoilYaw => maxRecoilYaw;

        public float GetRecoilScale(int shotIndex)
        {
            if (shotIndex < accurateRecoilShots)
                return accurateRecoilMultiplier;

            if (recoilRampShots <= 0)
                return 1f;

            float rampT = (shotIndex - accurateRecoilShots + 1f) / recoilRampShots;
            return Mathf.Lerp(accurateRecoilMultiplier, 1f, Mathf.Clamp01(rampT));
        }
    }
}
