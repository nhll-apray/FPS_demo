using UnityEngine;

namespace FpsDemo.Config.Enemy
{
    [CreateAssetMenu(fileName = "MeleeEnemyConfig", menuName = "Config/Enemy/Melee", order = 0)]
    public class MeleeEnemyConfig : ScriptableObject
    {
        [Header("Health")]
        [SerializeField] private int maxHealth = 150;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float acceleration = 12f;
        [SerializeField] private float angularSpeed = 720f;
        [SerializeField] private float stoppingDistance = 2f;

        [Header("AI")]
        [SerializeField] private float detectionRange = 25f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackCooldown = 1.25f;
        [SerializeField] private float repathInterval = 0.2f;
        [SerializeField] private float faceTargetSpeed = 12f;

        [Header("Melee Hit")]
        [SerializeField] private int attackDamage = 10;
        [SerializeField] private float attackHitRange = 2.2f;
        [SerializeField] private float attackHitAngle = 100f;

        public int MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float Acceleration => acceleration;
        public float AngularSpeed => angularSpeed;
        public float StoppingDistance => stoppingDistance;
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        public float AttackCooldown => attackCooldown;
        public float RepathInterval => repathInterval;
        public float FaceTargetSpeed => faceTargetSpeed;
        public int AttackDamage => attackDamage;
        public float AttackHitRange => attackHitRange;
        public float AttackHitAngle => attackHitAngle;
    }
}
