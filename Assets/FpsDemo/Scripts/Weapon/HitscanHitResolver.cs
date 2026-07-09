using System;
using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public sealed class HitscanHitResolver
    {
        private readonly int _hitMask;

        public HitscanHitResolver(int hitMask)
        {
            _hitMask = hitMask;
        }

        public bool TryResolve(Ray aimRay, float range, out HitscanHitResult hitResult)
        {
            hitResult = default;

            RaycastHit[] hits = Physics.RaycastAll(
                aimRay,
                range,
                _hitMask,
                QueryTriggerInteraction.Collide);

            if (hits.Length <= 0)
                return false;

            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            IDamageable selectedDamageable = null;
            for (int i = 0; i < hits.Length; i++)
            {
                if (!TryResolveDamageTarget(hits[i], out IDamageable damageable, out DamageHitZone hitZone))
                    continue;

                HitscanHitResult resolvedHit = new HitscanHitResult(
                    damageable,
                    hitZone,
                    hits[i].point,
                    hits[i].collider.gameObject);

                if (selectedDamageable == null)
                {
                    selectedDamageable = damageable;
                    hitResult = resolvedHit;

                    if (hitZone == DamageHitZone.Head)
                        return true;

                    continue;
                }

                if (!Equals(damageable, selectedDamageable))
                    break;

                if (hitZone == DamageHitZone.Head)
                {
                    hitResult = resolvedHit;
                    return true;
                }
            }

            return selectedDamageable != null;
        }

        private static bool TryResolveDamageTarget(
            RaycastHit hit,
            out IDamageable damageable,
            out DamageHitZone hitZone)
        {
            DamageHitbox hitbox = hit.collider.GetComponentInParent<DamageHitbox>();
            if (hitbox != null && hitbox.TryGetDamageable(out damageable))
            {
                hitZone = hitbox.HitZone;
                return true;
            }

            damageable = hit.collider.GetComponentInParent<IDamageable>();
            hitZone = DamageHitZone.Body;
            return damageable != null;
        }
    }
}
