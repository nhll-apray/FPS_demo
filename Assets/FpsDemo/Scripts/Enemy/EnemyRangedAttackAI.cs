using System.Collections;
using FpsDemo.Config;
using FpsDemo.Config.Enemy;
using FpsDemo.Game;
using FpsDemo.Player;
using UnityEngine;

namespace FpsDemo.Enemy
{
    public class EnemyRangedAttackAI : EnemyAIBase
    {
        [SerializeField] private RangedEnemyConfig data;
        [SerializeField] private Transform firePoint;

        private Coroutine _attackRoutine;
        private float _nextAttackTime;
        private bool _hasFiredThisAttack;
        private GameObject _projectilePrefab;

        private void Awake()
        {
            if (data == null)
            {
                data = GameResources.LoadConfig<RangedEnemyConfig>(GameResourcePaths.Config.Enemy.ElfRanged);
            }

            if (firePoint == null)
            {
                firePoint = FindChildByName(transform, "FirePoint");
            }

            firePoint ??= transform;

            InitializeEnemyAI(data, $"{nameof(EnemyRangedAttackAI)} could not load ranged enemy config.");
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

            if (sqrDistance > data.AttackRange * data.AttackRange)
            {
                EnterChase(data.StoppingDistance);
                return;
            }

            if (sqrDistance > data.PreferredRange * data.PreferredRange)
            {
                EnterChase(data.StoppingDistance);
                return;
            }

            if (!HasLineOfSightToTarget())
            {
                EnterChase(data.LostLineOfSightStoppingDistance);
                return;
            }

            EnterAttack();
        }

        private void EnterAttack()
        {
            SetAgentStoppingDistance(data.StoppingDistance);
            EnterAttackState();

            if (Time.time < _nextAttackTime || _attackRoutine != null)
            {
                return;
            }

            _attackRoutine = StartCoroutine(AttackRoutine());
            _nextAttackTime = Time.time + data.AttackCooldown;
        }

        private IEnumerator AttackRoutine()
        {
            _hasFiredThisAttack = false;
            TriggerAttackAnimation();

            if (data.AttackWindup > 0f)
            {
                yield return new WaitForSeconds(data.AttackWindup);
                TryFireProjectile();
            }
            else
            {
                TryFireProjectile();
            }

            _attackRoutine = null;
        }

        public void AnimationEvent_DealAttackDamage()
        {
            TryFireProjectile();
        }

        protected override void HandleDied()
        {
            if (_attackRoutine != null)
            {
                StopCoroutine(_attackRoutine);
                _attackRoutine = null;
            }

            base.HandleDied();
        }

        private void TryFireProjectile()
        {
            if (_hasFiredThisAttack)
            {
                return;
            }

            if (IsDead)
            {
                return;
            }

            if (Target == null || !IsTargetInAttackRange() || !HasLineOfSightToTarget())
            {
                return;
            }

            GameObject projectilePrefab = GetProjectilePrefab();
            if (projectilePrefab == null)
            {
                return;
            }

            _hasFiredThisAttack = true;

            Transform spawnPoint = firePoint != null ? firePoint : transform;
            Vector3 targetPoint = GetTargetLineOfSightPoint();
            Vector3 direction = targetPoint - spawnPoint.position;
            if (direction.sqrMagnitude <= 0.0001f)
            {
                direction = transform.forward;
            }

            GameObject projectileInstance = Instantiate(
                projectilePrefab,
                spawnPoint.position,
                Quaternion.LookRotation(direction.normalized));

            if (projectileInstance.TryGetComponent(out EnemyProjectile projectile))
            {
                projectile.Launch(gameObject, direction, data);
            }
        }

        private bool IsTargetInAttackRange()
        {
            return IsTargetInRange(data.AttackRange);
        }

        private bool HasLineOfSightToTarget()
        {
            if (Target == null)
            {
                return false;
            }

            Transform spawnPoint = firePoint != null ? firePoint : transform;
            Vector3 origin = spawnPoint.position;
            Vector3 targetPoint = GetTargetLineOfSightPoint();
            Vector3 toTarget = targetPoint - origin;
            float distance = toTarget.magnitude;
            if (distance <= 0.0001f)
            {
                return true;
            }

            Ray ray = new Ray(origin, toTarget / distance);
            if (!Physics.Raycast(ray, out RaycastHit hit, distance, data.LineOfSightMask, QueryTriggerInteraction.Ignore))
            {
                return true;
            }

            return IsTargetCollider(hit.collider);
        }

        private Vector3 GetTargetLineOfSightPoint()
        {
            return Target.position + Vector3.up * Mathf.Max(0f, data.LineOfSightTargetHeight);
        }

        private bool IsTargetCollider(Collider hitCollider)
        {
            if (hitCollider == null || Target == null)
            {
                return false;
            }

            if (hitCollider.transform == Target || hitCollider.transform.IsChildOf(Target))
            {
                return true;
            }

            PlayerEntity player = hitCollider.GetComponentInParent<PlayerEntity>();
            return player != null && player.transform == Target;
        }

        private GameObject GetProjectilePrefab()
        {
            return _projectilePrefab != null
                ? _projectilePrefab
                : _projectilePrefab = GameResources.LoadPrefab(data.ProjectilePrefabPath);
        }
    }
}
