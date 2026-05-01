using System;
using UnityEngine;

namespace FpsDemo.Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [field: SerializeField]
        public float MaxHealth {get; private set;}

        private float _currentHealth;
        public float CurrentHealth
        {
            get => _currentHealth;
            private set => _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        }

        private void Awake()
        {
            MaxHealth = 100f;
            CurrentHealth = MaxHealth;
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
