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
        public readonly int damage;

        public DamageInfo(int damage)
        {
            this.damage = damage;
        }
    }
}