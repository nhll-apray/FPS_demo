using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Player
{
    [RequireComponent(typeof(Health))]
    public class PlayerDeathHandler : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private bool restoreHealthOnDeath = true;

        private bool _isHandlingDeath;

        private void Awake()
        {
            if (health == null)
            {
                health = GetComponent<Health>();
            }
        }

        private void OnEnable()
        {
            if (health != null)
            {
                health.OnDied += HandleDied;
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.OnDied -= HandleDied;
            }
        }

        private void HandleDied()
        {
            if (_isHandlingDeath)
            {
                return;
            }

            _isHandlingDeath = true;

            if (restoreHealthOnDeath && health != null)
            {
                health.ResetHealth();
            }

            _isHandlingDeath = false;
        }
    }
}
