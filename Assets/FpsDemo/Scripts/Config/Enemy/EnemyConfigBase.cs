using UnityEngine;

namespace FpsDemo.Config.Enemy
{
    public abstract class EnemyConfigBase : ScriptableObject
    {
        [SerializeField] private int maxHealth = 100;

        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float angularSpeed = 540f;
        [SerializeField] private float stoppingDistance = 2f;

        [SerializeField] private float detectionRange = 25f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackCooldown = 1.25f;
        [SerializeField] private float repathInterval = 0.2f;
        [SerializeField] private float faceTargetSpeed = 12f;

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
    }
}
