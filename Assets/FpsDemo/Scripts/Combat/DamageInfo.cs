using UnityEngine;

namespace FpsDemo.Combat
{
    public enum DamageType
    {
        Hitscan,
        Projectile,
        Explosion,
        Melee
    }

    public enum DamageHitZone
    {
        Body,
        Head
    }

    public readonly struct DamageInfo
    {
        public readonly int Damage;
        public readonly GameObject Attacker;
        public readonly DamageType DamageType;
        public readonly Vector3 HitPoint;
        public readonly DamageHitZone HitZone;
        public bool IsHeadshot => HitZone == DamageHitZone.Head;
        
        public DamageInfo(int damage, GameObject attacker)
            : this(damage, attacker, DamageType.Hitscan, Vector3.zero, DamageHitZone.Body)
        {
        }

        public DamageInfo(int damage, GameObject attacker, DamageType damageType, Vector3 hitPoint)
            : this(damage, attacker, damageType, hitPoint, DamageHitZone.Body)
        {
        }

        public DamageInfo(int damage, GameObject attacker, DamageType damageType, Vector3 hitPoint, DamageHitZone hitZone)
        {
            this.Damage = damage;
            this.Attacker = attacker;
            this.DamageType = damageType;
            this.HitPoint = hitPoint;
            this.HitZone = hitZone;
        }
    }

    public readonly struct DamageResult
    {
        public static readonly DamageResult None = new DamageResult(0, null, null, false);
        
        public readonly int DamageApplied;
        public readonly GameObject Attacker;
        public readonly GameObject Target;
        public readonly bool IsKill;
        public readonly bool IsHeadshot;
        
        public DamageResult(int damage, GameObject attacker, GameObject target, bool isKill, bool isHeadshot = false)
        {
            this.DamageApplied = damage;
            this.Attacker = attacker;
            this.Target = target;
            this.IsKill = isKill;
            this.IsHeadshot = isHeadshot;
        }
    }
}
