using UnityEngine;

namespace FpsDemo.Config.Enemy
{
    [CreateAssetMenu(fileName = "RangedEnemyConfig", menuName = "Config/Enemy/Ranged", order = 0)]
    public class RangedEnemyConfig : ScriptableObject
    {
        [Header("Health")]
        [SerializeField] private int maxHealth = 100;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float angularSpeed = 540f;
        [SerializeField] private float stoppingDistance = 8f;

        [Header("AI")]
        [SerializeField] private float detectionRange = 28f;
        [SerializeField] private float attackRange = 14f;
        [SerializeField] private float preferredRange = 9f;
        [SerializeField] private float attackCooldown = 1.8f;
        [SerializeField] private float attackWindup = 0.6f;
        [SerializeField] private float repathInterval = 0.2f;
        [SerializeField] private float faceTargetSpeed = 12f;
        [SerializeField] private LayerMask lineOfSightMask = Physics.DefaultRaycastLayers;
        [SerializeField] private float lineOfSightTargetHeight = 1.2f;
        [SerializeField] private float lostLineOfSightStoppingDistance = 1.5f;

        [Header("Projectile")]
        [SerializeField] private int projectileDamage = 12;
        [SerializeField] private float projectileSpeed = 12f;
        [SerializeField] private float projectileRadius = 0.25f;
        [SerializeField] private float projectileLifetime = 4f;
        [SerializeField] private LayerMask projectileHitMask = ~0;
        [SerializeField] private string projectilePrefabPath = GameResourcePaths.Prefabs.Projectiles.EnemyEnergyBall;

        public int MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float Acceleration => acceleration;
        public float AngularSpeed => angularSpeed;
        public float StoppingDistance => stoppingDistance;
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        public float PreferredRange => preferredRange;
        public float AttackCooldown => attackCooldown;
        public float AttackWindup => attackWindup;
        public float RepathInterval => repathInterval;
        public float FaceTargetSpeed => faceTargetSpeed;
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
