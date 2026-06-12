using System.Collections;
using System.Collections.Generic;
using FpsDemo.Config.Weapon;
using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class GrenadeProjectile : MonoBehaviour
    {
        private const int DamageOverlapBufferSize = 32;
        private const int PhysicsOverlapBufferSize = 64;

        [SerializeField] private AudioSource impactSound;

        private readonly HashSet<IDamageable> _damagedTargets = new HashSet<IDamageable>();
        private readonly Collider[] _damageOverlapBuffer = new Collider[DamageOverlapBufferSize];
        private readonly Collider[] _physicsOverlapBuffer = new Collider[PhysicsOverlapBufferSize];

        private GameObject _owner;
        private GrenadeAltFireConfig _config;
        private Rigidbody _rigidbody;
        private Coroutine _fuseCoroutine;
        private bool _hasExploded;
        private GameObject _explosionPrefab;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Launch(GameObject owner, Vector3 direction, GrenadeAltFireConfig config)
        {
            if (config == null)
                return;

            _owner = owner;
            _config = config;

            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = direction.normalized * _config.throwForce + Vector3.up * _config.upwardForce;
                _rigidbody.AddRelativeTorque(Random.Range(500f, 1500f), 0f, 0f);
            }

            StartFuse(_config.fuseTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (impactSound != null)
            {
                impactSound.Play();
            }
        }

        private void StartFuse(float fuseTime)
        {
            if (_fuseCoroutine != null)
            {
                StopCoroutine(_fuseCoroutine);
            }

            _fuseCoroutine = StartCoroutine(FuseRoutine(fuseTime));
        }

        private IEnumerator FuseRoutine(float fuseTime)
        {
            yield return new WaitForSeconds(Mathf.Max(0f, fuseTime));
            Explode();
        }

        private void Explode()
        {
            if (_hasExploded || _config == null)
                return;

            _hasExploded = true;
            SpawnExplosionVisual();
            ApplyAreaDamage();
            ApplyPhysicsExplosion();
            Destroy(gameObject);
        }

        private void SpawnExplosionVisual()
        {
            GameObject explosionPrefab = GetExplosionPrefab();
            if (explosionPrefab == null)
                return;

            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        private void ApplyAreaDamage()
        {
            if (_config.damage <= 0 || _config.radius <= 0f)
                return;

            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _config.radius, _damageOverlapBuffer, _config.damageMask, QueryTriggerInteraction.Ignore);

            _damagedTargets.Clear();
            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = _damageOverlapBuffer[i];
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                if (damageable == null || !_damagedTargets.Add(damageable))
                    continue;

                Vector3 hitPoint = hit.ClosestPoint(transform.position);
                DamageSystem.ApplyDamage(damageable, new DamageInfo(_config.damage, _owner, DamageType.Explosion, hitPoint));
            }
        }

        private void ApplyPhysicsExplosion()
        {
            if (!_config.applyExplosionForce || _config.radius <= 0f)
                return;

            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _config.radius, _physicsOverlapBuffer, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = _physicsOverlapBuffer[i];
                Rigidbody hitRigidbody = hit.attachedRigidbody;
                if (hitRigidbody != null)
                {
                    hitRigidbody.AddExplosionForce(_config.explosionForce, transform.position, _config.radius, _config.explosionUpwardsModifier);
                }
            }
        }

        private GameObject GetExplosionPrefab()
        {
            return _explosionPrefab != null
                ? _explosionPrefab
                : _explosionPrefab = GameResources.LoadPrefab(_config.ExplosionPrefabPath);
        }
    }
}
