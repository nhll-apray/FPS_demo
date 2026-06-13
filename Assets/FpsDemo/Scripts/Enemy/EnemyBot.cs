using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Enemy
{
    public class EnemyBot : MonoBehaviour
    {
        [SerializeField] private float destroyDelayAfterDeath = 2f;
        [SerializeField] private bool disableCollidersOnDeath = true;

        private Health _health;
        private Collider[] _colliders;
        private bool _isDying;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _colliders = GetComponentsInChildren<Collider>();
        }

        private void OnEnable()
        {
            if (_health != null)
            {
                _health.OnDied += Die;
            }
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.OnDied -= Die;
            }
        }

        private void Die()
        {
            if (_isDying)
            {
                return;
            }

            _isDying = true;
            DisableDamageColliders();
            Destroy(gameObject, Mathf.Max(0f, destroyDelayAfterDeath));
        }

        private void DisableDamageColliders()
        {
            if (!disableCollidersOnDeath || _colliders == null)
            {
                return;
            }

            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i] != null)
                {
                    _colliders[i].enabled = false;
                }
            }
        }
    }
}
