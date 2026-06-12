using UnityEngine;

namespace FpsDemo.Enemy
{
    public static class EnemyDamageRules
    {
        public static bool IsFriendlyFire(GameObject attacker, Collider hitCollider)
        {
            if (hitCollider == null)
            {
                return false;
            }

            return IsFriendlyFire(attacker, hitCollider.transform);
        }

        public static bool IsFriendlyFire(GameObject attacker, Transform target)
        {
            if (attacker == null || target == null)
            {
                return false;
            }

            EnemyBot attackerEnemy = attacker.GetComponentInParent<EnemyBot>();
            EnemyBot targetEnemy = target.GetComponentInParent<EnemyBot>();
            return attackerEnemy != null && targetEnemy != null && attackerEnemy != targetEnemy;
        }
    }
}
