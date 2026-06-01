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

            if (result.damageApplied > 0)
            {
                EventManager.Broadcast(new DamageDealtEvent{ damageResult =  result });
            }

            return result;
        }
    }
}