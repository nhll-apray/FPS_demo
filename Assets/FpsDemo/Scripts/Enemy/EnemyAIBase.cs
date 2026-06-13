using FpsDemo.Combat;
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
    public abstract class EnemyAIBase : MonoBehaviour
    {
        protected enum EnemyAIState
        {
            Idle,
            Chase,
            Attack,
            Dead
        }

        protected static readonly int TriggerAttack = Animator.StringToHash("Attack");
        protected static readonly int TriggerDie = Animator.StringToHash("Die");
        protected static readonly int FloatMoveSpeed = Animator.StringToHash("MoveSpeed");

        protected NavMeshAgent Agent { get; private set; }
        protected Health Health { get; private set; }
        protected Animator Animator { get; private set; }
        protected Transform Target { get; private set; }
        protected EnemyConfigBase Config { get; private set; }
        protected EnemyAIState State { get; private set; }

        protected bool IsDead => Health != null && Health.IsDead;
        protected bool CanUseAgent => Agent != null && Agent.enabled && Agent.isOnNavMesh;

        protected void InitializeEnemyAI(EnemyConfigBase config, string missingConfigMessage)
        {
            Agent = GetComponent<NavMeshAgent>();
            Health = GetComponent<Health>();
            Animator = GetComponent<Animator>();
            Config = config;

            if (Config == null)
            {
                Debug.LogError(missingConfigMessage, this);
                enabled = false;
                return;
            }

            ApplyStats();
        }

        protected virtual void OnEnable()
        {
            if (Health != null)
            {
                Health.OnDied += HandleDied;
            }
        }

        protected virtual void OnDisable()
        {
            if (Health != null)
            {
                Health.OnDied -= HandleDied;
            }
        }

        protected bool TryAcquireTarget()
        {
            if (Target != null)
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

            Target = player.transform;
            return Target != null;
        }

        protected void EnterIdle()
        {
            SetState(EnemyAIState.Idle);
            StopAgent(clearPath: true);
            SetMoveSpeed(0f);
        }

        protected void EnterChase(float stoppingDistance)
        {
            SetState(EnemyAIState.Chase);

            if (!CanUseAgent)
            {
                FaceTarget();
                return;
            }

            SetAgentStoppingDistance(stoppingDistance);
            Agent.isStopped = false;
            SetMoveSpeed(Agent.velocity.magnitude);

            if (!Agent.hasPath || Time.time >= NextPathUpdateTime)
            {
                Agent.SetDestination(Target.position);
                NextPathUpdateTime = Time.time + Config.RepathInterval;
            }
        }

        protected bool EnterAttackState()
        {
            bool changedState = SetState(EnemyAIState.Attack);
            StopAgent(clearPath: false);
            SetMoveSpeed(0f);
            FaceTarget();
            return changedState;
        }

        protected virtual void HandleDied()
        {
            SetState(EnemyAIState.Dead);
            StopAgent(clearPath: true);
            SetMoveSpeed(0f);
            Animator.SetTrigger(TriggerDie);
        }

        protected void StopAgent(bool clearPath)
        {
            if (!CanUseAgent)
            {
                return;
            }

            Agent.isStopped = true;
            if (clearPath && Agent.hasPath)
            {
                Agent.ResetPath();
            }
        }

        protected void FaceTarget()
        {
            if (Target == null)
            {
                return;
            }

            Vector3 direction = Target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Config.FaceTargetSpeed * Time.deltaTime);
        }

        protected float GetPlanarSqrDistance(Vector3 position)
        {
            Vector3 offset = position - transform.position;
            offset.y = 0f;
            return offset.sqrMagnitude;
        }

        protected bool IsTargetInRange(float range)
        {
            return Target != null && GetPlanarSqrDistance(Target.position) <= range * range;
        }

        protected void TriggerAttackAnimation()
        {
            Animator.SetTrigger(TriggerAttack);
        }

        protected void SetAgentStoppingDistance(float stoppingDistance)
        {
            if (Agent != null)
            {
                Agent.stoppingDistance = Mathf.Max(0f, stoppingDistance);
            }
        }

        protected static Transform FindChildByName(Transform root, string childName)
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

        private float NextPathUpdateTime { get; set; }

        private bool SetState(EnemyAIState state)
        {
            if (State == state)
            {
                return false;
            }

            State = state;
            return true;
        }

        private void SetMoveSpeed(float moveSpeed)
        {
            Animator.SetFloat(FloatMoveSpeed, moveSpeed);
        }

        private void ApplyStats()
        {
            if (Health != null)
            {
                Health.SetMaxHealth(Config.MaxHealth);
            }

            if (Agent == null)
            {
                return;
            }

            Agent.speed = Config.MoveSpeed;
            Agent.acceleration = Config.Acceleration;
            Agent.angularSpeed = Config.AngularSpeed;
            SetAgentStoppingDistance(Config.StoppingDistance);
        }
    }
}
