using FpsDemo.Combat;
using FpsDemo.Config.Enemy;
using UnityEngine;

namespace FpsDemo.Enemy
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float fallbackSpeed = 12f;
        [SerializeField] private float fallbackRadius = 0.25f;
        [SerializeField] private float fallbackLifetime = 4f;
        [SerializeField] private int fallbackDamage = 10;
        [SerializeField] private LayerMask fallbackHitMask = ~0;

        private GameObject _owner;
        private Vector3 _direction;
        private float _speed;
        private float _radius;
        private float _lifeEndTime;
        private int _damage;
        private LayerMask _hitMask;
        private bool _isInitialized;

        public void Launch(GameObject owner, Vector3 direction, RangedEnemyConfig data)
        {
            _owner = owner;
            _direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;

            _speed = data != null ? data.ProjectileSpeed : fallbackSpeed;
            _radius = data != null ? data.ProjectileRadius : fallbackRadius;
            _damage = data != null ? data.ProjectileDamage : fallbackDamage;
            _hitMask = data != null ? data.ProjectileHitMask : fallbackHitMask;

            float lifetime = data != null ? data.ProjectileLifetime : fallbackLifetime;
            _lifeEndTime = Time.time + Mathf.Max(0.1f, lifetime);
            _isInitialized = true;

            transform.rotation = Quaternion.LookRotation(_direction);
        }

        private void Awake()
        {
            if (!_isInitialized)
            {
                Launch(null, transform.forward, null);
            }
        }

        private void Update()
        {
            if (Time.time >= _lifeEndTime)
            {
                Destroy(gameObject);
                return;
            }

            float distance = _speed * Time.deltaTime;
            if (TryHit(distance, out RaycastHit hit))
            {
                HandleHit(hit);
                return;
            }

            transform.position += _direction * distance;
        }

        private bool TryHit(float distance, out RaycastHit hit)
        {
            return Physics.SphereCast(
                transform.position,
                _radius,
                _direction,
                out hit,
                distance,
                _hitMask,
                QueryTriggerInteraction.Ignore);
        }

        private void HandleHit(RaycastHit hit)
        {
            if (_owner != null && hit.transform.IsChildOf(_owner.transform))
            {
                transform.position = hit.point + _direction * (_radius + 0.01f);
                return;
            }

            if (EnemyDamageRules.IsFriendlyFire(_owner, hit.collider))
            {
                Destroy(gameObject);
                return;
            }

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                DamageSystem.ApplyDamage(damageable, new DamageInfo(_damage, _owner, DamageType.Projectile, hit.point));
            }

            Destroy(gameObject);
        }
    }
}
