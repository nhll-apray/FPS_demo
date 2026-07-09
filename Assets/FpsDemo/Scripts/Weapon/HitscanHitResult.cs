using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public readonly struct HitscanHitResult
    {
        public readonly IDamageable Damageable;
        public readonly DamageHitZone HitZone;
        public readonly Vector3 HitPoint;
        public readonly GameObject HitObject;

        public HitscanHitResult(
            IDamageable damageable,
            DamageHitZone hitZone,
            Vector3 hitPoint,
            GameObject hitObject)
        {
            Damageable = damageable;
            HitZone = hitZone;
            HitPoint = hitPoint;
            HitObject = hitObject;
        }
    }
}
