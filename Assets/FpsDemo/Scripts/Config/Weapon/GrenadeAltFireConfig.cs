using UnityEngine;

namespace FpsDemo.Config.Weapon
{
    [CreateAssetMenu(fileName = "GrenadeAltFireConfig", menuName = "Config/Weapon/Alt Fire/Grenade")]
    public class GrenadeAltFireConfig : ScriptableObject
    {
        public int damage = 80;
        public float radius = 5f;
        public float fuseTime = 2f;
        public float throwForce = 16f;
        public float upwardForce = 3f;
        public bool applyExplosionForce = true;
        public float explosionForce = 850f;
        public float explosionUpwardsModifier = 3f;
        public float cooldown = 1.2f;
        public float releaseDelay = 0.35f;
        public float finishDelay = 0.8f;
        public LayerMask damageMask;

        [SerializeField] private string grenadePrefabPath = GameResourcePaths.Prefabs.Projectiles.HandGrenade;
        [SerializeField] private string explosionPrefabPath = GameResourcePaths.Prefabs.VFX.Explosion;

        public string GrenadePrefabPath => grenadePrefabPath;
        public string ExplosionPrefabPath => explosionPrefabPath;
    }
}
