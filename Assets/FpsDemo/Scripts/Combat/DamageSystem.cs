using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Combat
{
    public static class DamageSystem
    {
        public static DamageResult ApplyDamage(IDamageable target, DamageInfo info)
        {
            if (target == null)
            {
                return DamageResult.None;
            }
            
            DamageResult result = target.TakeDamage(info);

            if (result.DamageApplied > 0)
            {
                EventManager.Broadcast(new DamageDealtEvent{ DamageResult =  result });
            }

            return result;
        }
    }
}