using System;
using UnityEngine;

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
                OnHealthChanged?.Invoke(healthAfter, MaxHealth);
                currentHealth = healthAfter;
                if (currentHealth <= 0)
                {
                    OnDied?.Invoke();
                }
            }
        }
        
        public bool IsDead => CurrentHealth <= 0;
        
        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;

        public DamageResult TakeDamage(DamageInfo damageInfo)
        {
            if (IsDead)
                return DamageResult.None;
            int healthBefore = CurrentHealth;
            CurrentHealth -= damageInfo.damage;
            int damage = Math.Max(0, healthBefore - CurrentHealth) ;
            DamageResult damageResult = new DamageResult(damage, damageInfo.attacker, gameObject, IsDead);
            return damageResult;
        }
        

        public void ResetHealth()
        {
            currentHealth = MaxHealth;
        }
    }
}
