using FpsDemo.Combat;
using FpsDemo.Config;
using FpsDemo.Config.Enemy;
using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Enemy
{
    public class EnemyMeleeAttackAI : EnemyAIBase
    {
        [SerializeField] private MeleeEnemyConfig data;

        private float _nextAttackTime;

        private void Awake()
        {
            if (data == null)
            {
                data = GameResources.LoadConfig<MeleeEnemyConfig>(GameResourcePaths.Config.Enemy.GoblinMelee);
            }

            InitializeEnemyAI(data, $"{nameof(EnemyMeleeAttackAI)} could not load melee enemy config.");
        }

        private void Update()
        {
            if (IsDead)
            {
                return;
            }

            if (!TryAcquireTarget())
            {
                EnterIdle();
                return;
            }

            float sqrDistance = GetPlanarSqrDistance(Target.position);
            if (sqrDistance > data.DetectionRange * data.DetectionRange)
            {
                EnterIdle();
                return;
            }

            if (sqrDistance <= data.AttackRange * data.AttackRange)
            {
                EnterAttack();
                return;
            }

            EnterChase(data.StoppingDistance);
        }

        private void EnterAttack()
        {
            bool enteredAttack = EnterAttackState();
            if (enteredAttack)
            {
                _nextAttackTime = 0f;
            }

            if (Time.time < _nextAttackTime)
            {
                return;
            }

            PerformAttack();
            _nextAttackTime = Time.time + data.AttackCooldown;
        }

        private void PerformAttack()
        {
            if (Target == null)
            {
                return;
            }

            TriggerAttackAnimation();
        }

        public void AnimationEvent_DealAttackDamage()
        {
            if (Target == null)
            {
                return;
            }

            Vector3 toTarget = Target.position - transform.position;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude > data.AttackHitRange * data.AttackHitRange)
            {
                return;
            }

            float minDot = Mathf.Cos(data.AttackHitAngle * 0.5f * Mathf.Deg2Rad);
            if (Vector3.Dot(transform.forward, toTarget.normalized) < minDot)
            {
                return;
            }

            if (EnemyDamageRules.IsFriendlyFire(gameObject, Target))
            {
                return;
            }

            IDamageable damageable = Target.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                DamageSystem.ApplyDamage(damageable, new DamageInfo(data.AttackDamage, gameObject, DamageType.Melee, Target.position));
            }
        }
    }
}
