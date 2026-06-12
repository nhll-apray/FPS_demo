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
    public class EnemyChaseAttackAI : MonoBehaviour
    {
        [SerializeField] private MeleeEnemyConfig data;

        private NavMeshAgent _agent;
        private Health _health;
        private Transform _target;
        private EnemyAIState _state;
        private Animator _animator;
        private float _nextPathUpdateTime;
        private float _nextAttackTime;

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
                data = GameResources.LoadConfig<MeleeEnemyConfig>(GameResourcePaths.Config.Enemy.GoblinMelee);
            }

            if (data == null)
            {
                Debug.LogError($"{nameof(EnemyChaseAttackAI)} could not load melee enemy data.", this);
                enabled = false;
                return;
            }

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

            if (sqrDistance <= data.AttackRange * data.AttackRange)
            {
                EnterAttack();
                return;
            }

            EnterChase();
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

        private void EnterChase()
        {
            SetState(EnemyAIState.Chase);

            if (!CanUseAgent)
            {
                FaceTarget();
                return;
            }

            _agent.isStopped = false;
            
            _animator.SetFloat(FloatMoveSpeed, _agent.velocity.magnitude);
            
            if (Time.time >= _nextPathUpdateTime)
            {
                _agent.SetDestination(_target.position);
                _nextPathUpdateTime = Time.time + data.RepathInterval;
            }
        }

        private void EnterAttack()
        {
            SetState(EnemyAIState.Attack);
            StopAgent(clearPath: false);
            _animator.SetFloat(FloatMoveSpeed, 0f);
            FaceTarget();

            if (Time.time < _nextAttackTime)
            {
                return;
            }

            PerformAttack();
            _nextAttackTime = Time.time + data.AttackCooldown;
        }

        private void PerformAttack()
        {
            if (_target == null)
            {
                return;
            }
            
            _animator.SetTrigger(TriggerAttack);

            Debug.Log($"{name} attacks {_target.name}.", this);
        }

        private void HandleDied()
        {
            SetState(EnemyAIState.Dead);
            StopAgent(clearPath: true);
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

        private void SetState(EnemyAIState state)
        {
            if (_state == state)
            {
                return;
            }

            _state = state;
            if (_state == EnemyAIState.Attack)
            {
                _nextAttackTime = 0f;
            }
        }
        
        public void AnimationEvent_DealAttackDamage()
        {
            if (_target == null)
                return;

            Vector3 toTarget = _target.position - transform.position;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude > data.AttackHitRange * data.AttackHitRange)
                return;

            float minDot = Mathf.Cos(data.AttackHitAngle * 0.5f * Mathf.Deg2Rad);
            if (Vector3.Dot(transform.forward, toTarget.normalized) < minDot)
                return;

            if (EnemyDamageRules.IsFriendlyFire(gameObject, _target))
                return;

            IDamageable damageable = _target.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                DamageSystem.ApplyDamage(damageable, new DamageInfo(data.AttackDamage, gameObject, DamageType.Melee, _target.position));
            }
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
            _agent.stoppingDistance = data.StoppingDistance;
        }
    }
}
