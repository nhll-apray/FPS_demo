using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FpsDemo.Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [field: SerializeField]
        public int MaxHealth {get; private set;}

        [SerializeField]
        private int currentHealth;
        public int CurrentHealth
        {
            get => currentHealth;
            private set
            {
                int healthAfter = Mathf.Clamp(value, 0, MaxHealth);
                onHealthChanged(healthAfter, MaxHealth);
                currentHealth = healthAfter;
                if (currentHealth <= 0)
                {
                    died.Invoke();
                }
            }
        }
        
        public bool IsDead => CurrentHealth <= 0;
        
        public Action<int, int> onHealthChanged;
        public Action damaged;
        public Action died;

        private void Awake()
        {

        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            CurrentHealth -= damageInfo.damage;
            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
