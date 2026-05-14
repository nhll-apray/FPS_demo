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
        public readonly GameObject attacker;
        
        public DamageInfo(int damage, GameObject attacker)
        {
            this.damage = damage;
            this.attacker = attacker;
        }
    }

    public readonly struct DamageResult
    {
        public static readonly DamageResult None = new DamageResult(0, null, null, false);
        
        public readonly int damageApplied;
        public readonly GameObject attacker;
        public readonly GameObject target;
        public readonly bool isKill;
        public readonly bool isHeadshot;
        
        public DamageResult(int damage, GameObject attacker, GameObject target, bool isKill, bool isHeadshot = false)
        {
            this.damageApplied = damage;
            this.attacker = attacker;
            this.target = target;
            this.isKill = isKill;
            this.isHeadshot = isHeadshot;
        }
    }
}