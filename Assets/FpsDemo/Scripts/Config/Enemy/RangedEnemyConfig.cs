using UnityEngine;

namespace FpsDemo.Config.Enemy
{
    [CreateAssetMenu(fileName = "RangedEnemyConfig", menuName = "Config/Enemy/Ranged", order = 0)]
    public class RangedEnemyConfig : EnemyConfigBase
    {
        [SerializeField] private float preferredRange = 9f;
        [SerializeField] private float attackWindup = 0.6f;
        [SerializeField] private LayerMask lineOfSightMask = Physics.DefaultRaycastLayers;
        [SerializeField] private float lineOfSightTargetHeight = 1.2f;
        [SerializeField] private float lostLineOfSightStoppingDistance = 1.5f;
        
        [SerializeField] private int projectileDamage = 12;
        [SerializeField] private float projectileSpeed = 12f;
        [SerializeField] private float projectileRadius = 0.25f;
        [SerializeField] private float projectileLifetime = 4f;
        [SerializeField] private LayerMask projectileHitMask = ~0;
        [SerializeField] private string projectilePrefabPath = GameResourcePaths.Prefabs.Projectiles.EnemyEnergyBall;

        public float PreferredRange => preferredRange;
        public float AttackWindup => attackWindup;
        public LayerMask LineOfSightMask => lineOfSightMask;
        public float LineOfSightTargetHeight => lineOfSightTargetHeight;
        public float LostLineOfSightStoppingDistance => lostLineOfSightStoppingDistance;
        public int ProjectileDamage => projectileDamage;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileRadius => projectileRadius;
        public float ProjectileLifetime => projectileLifetime;
        public LayerMask ProjectileHitMask => projectileHitMask;
        public string ProjectilePrefabPath => projectilePrefabPath;
    }
}
