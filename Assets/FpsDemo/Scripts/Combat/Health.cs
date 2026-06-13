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
                currentHealth = healthAfter;
                OnHealthChanged?.Invoke(currentHealth, MaxHealth);
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
            int preHealth = CurrentHealth;
            int healthAfterDamage = Mathf.Clamp(CurrentHealth - damageInfo.Damage, 0, MaxHealth);
            int damage = Math.Max(0, preHealth - healthAfterDamage);
            bool isKilled = healthAfterDamage <= 0;

            CurrentHealth = healthAfterDamage;
            DamageResult damageResult = new DamageResult(damage, damageInfo.Attacker, gameObject, isKilled);
            return damageResult;
        }
        

        public void ResetHealth()
        {
            CurrentHealth = MaxHealth;
        }

        public void SetMaxHealth(int maxHealth, bool resetCurrentHealth = true)
        {
            MaxHealth = Mathf.Max(1, maxHealth);
            CurrentHealth = resetCurrentHealth
                ? MaxHealth
                : Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        }
    }
}
