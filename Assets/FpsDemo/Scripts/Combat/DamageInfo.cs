using UnityEngine;

namespace FpsDemo.Combat
{
    public enum DamageType
    {
        Hitscan,
        Projectile,
        Explosion
    }
    public readonly struct DamageInfo
    {
        public readonly float damage;

        public DamageInfo(float damage)
        {
            this.damage = damage;
        }
    }
}