using System.Collections;
using FpsDemo.Combat;
using FpsDemo.Config;
using FpsDemo.Config.Enemy;
using FpsDemo.Game;
using FpsDemo.Player;
using UnityEngine;
using UnityEngine.AI;

namespace FpsDemo.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Animator))]
    public class EnemyRangedAttackAI : MonoBehaviour
    {
        [SerializeField] private RangedEnemyConfig data;
        [SerializeField] private Transform firePoint;

        private NavMeshAgent _agent;
        private Health _health;
        private Animator _animator;
        private Transform _target;
        private EnemyAIState _state;
        private Coroutine _attackRoutine;
        private float _nextPathUpdateTime;
        private float _nextAttackTime;
        private bool _hasFiredThisAttack;
        private GameObject _projectilePrefab;

        private bool CanUseAgent => _agent != null && _agent.enabled && _agent.isOnNavMesh;

        private enum EnemyAIState
        {
            Idle,
            Chase,
            Attack,
            Dead
        }

        private static readonly int TriggerAttack = Animator.StringToHash("Attack");
        private static readonly int TriggerDie = Animator.StringToHash("Die");
        private static readonly int FloatMoveSpeed = Animator.StringToHash("MoveSpeed");

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
            _animator = GetComponent<Animator>();

            if (data == null)
            {
                data = GameResources.LoadConfig<RangedEnemyConfig>(GameResourcePaths.Config.Enemy.ElfRanged);
            }

            if (data == null)
            {
                Debug.LogError($"{nameof(EnemyRangedAttackAI)} could not load ranged enemy data.", this);
                enabled = false;
                return;
            }

            if (firePoint == null)
            {
                firePoint = FindChildByName(transform, "FirePoint");
            }

            firePoint ??= transform;

            ApplyStats();
        }

        private void OnEnable()
        {
            if (_health != null)
            {
                _health.OnDied += HandleDied;
            }
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.OnDied -= HandleDied;
            }
        }

        private void Update()
        {
            if (_health != null && _health.IsDead)
            {
                return;
            }

            if (!TryAcquireTarget())
            {
                EnterIdle();
                return;
            }

            float sqrDistance = GetPlanarSqrDistance(_target.position);
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

        private bool TryAcquireTarget()
        {
            if (_target != null)
            {
                return true;
            }

            PlayerEntity player = GameManager.Instance != null
                ? GameManager.Instance.CurrentPlayer
                : null;

            if (player == null)
            {
                player = Object.FindFirstObjectByType<PlayerEntity>();
            }

            if (player == null)
            {
                return false;
            }

            _target = player.transform;
            return _target != null;
        }

        private void EnterIdle()
        {
            SetState(EnemyAIState.Idle);
            StopAgent(clearPath: true);
            _animator.SetFloat(FloatMoveSpeed, 0f);
        }

        private void EnterChase(float stoppingDistance)
        {
            SetState(EnemyAIState.Chase);

            if (!CanUseAgent)
            {
                FaceTarget();
                return;
            }

            _agent.stoppingDistance = Mathf.Max(0f, stoppingDistance);
            _agent.isStopped = false;
            _animator.SetFloat(FloatMoveSpeed, _agent.velocity.magnitude);

            if (!_agent.hasPath || Time.time >= _nextPathUpdateTime)
            {
                _agent.SetDestination(_target.position);
                _nextPathUpdateTime = Time.time + data.RepathInterval;
            }
        }

        private void EnterAttack()
        {
            SetState(EnemyAIState.Attack);
            SetAgentStoppingDistance(data.StoppingDistance);
            StopAgent(clearPath: false);
            _animator.SetFloat(FloatMoveSpeed, 0f);
            FaceTarget();

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
            _animator.SetTrigger(TriggerAttack);

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

        public void AnimationEvent_FireProjectile()
        {
            TryFireProjectile();
        }

        public void AnimationEvent_DealAttackDamage()
        {
            TryFireProjectile();
        }

        private void TryFireProjectile()
        {
            if (_hasFiredThisAttack)
            {
                return;
            }

            if (_health != null && _health.IsDead)
            {
                return;
            }

            if (_target == null || !IsTargetInAttackRange() || !HasLineOfSightToTarget())
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

        private void HandleDied()
        {
            SetState(EnemyAIState.Dead);
            StopAgent(clearPath: true);
            _animator.SetTrigger(TriggerDie);
        }

        private void StopAgent(bool clearPath)
        {
            if (!CanUseAgent)
            {
                return;
            }

            _agent.isStopped = true;
            if (clearPath && _agent.hasPath)
            {
                _agent.ResetPath();
            }
        }

        private void FaceTarget()
        {
            if (_target == null)
            {
                return;
            }

            Vector3 direction = _target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, data.FaceTargetSpeed * Time.deltaTime);
        }

        private float GetPlanarSqrDistance(Vector3 position)
        {
            Vector3 offset = position - transform.position;
            offset.y = 0f;
            return offset.sqrMagnitude;
        }

        private bool IsTargetInAttackRange()
        {
            if (_target == null)
            {
                return false;
            }

            return GetPlanarSqrDistance(_target.position) <= data.AttackRange * data.AttackRange;
        }

        private bool HasLineOfSightToTarget()
        {
            if (_target == null)
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
            return _target.position + Vector3.up * Mathf.Max(0f, data.LineOfSightTargetHeight);
        }

        private bool IsTargetCollider(Collider hitCollider)
        {
            if (hitCollider == null || _target == null)
            {
                return false;
            }

            if (hitCollider.transform == _target || hitCollider.transform.IsChildOf(_target))
            {
                return true;
            }

            PlayerEntity player = hitCollider.GetComponentInParent<PlayerEntity>();
            return player != null && player.transform == _target;
        }

        private static Transform FindChildByName(Transform root, string childName)
        {
            if (root == null)
            {
                return null;
            }

            Transform[] children = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].name == childName)
                {
                    return children[i];
                }
            }

            return null;
        }

        private void SetState(EnemyAIState state)
        {
            if (_state == state)
            {
                return;
            }

            _state = state;
        }

        private void ApplyStats()
        {
            if (_health != null)
            {
                _health.SetMaxHealth(data.MaxHealth);
            }

            if (_agent == null)
            {
                return;
            }

            _agent.speed = data.MoveSpeed;
            _agent.acceleration = data.Acceleration;
            _agent.angularSpeed = data.AngularSpeed;
            SetAgentStoppingDistance(data.StoppingDistance);
        }

        private void SetAgentStoppingDistance(float stoppingDistance)
        {
            if (_agent != null)
            {
                _agent.stoppingDistance = Mathf.Max(0f, stoppingDistance);
            }
        }

        private GameObject GetProjectilePrefab()
        {
            return _projectilePrefab != null
                ? _projectilePrefab
                : _projectilePrefab = GameResources.LoadPrefab(data.ProjectilePrefabPath);
        }
    }
}
