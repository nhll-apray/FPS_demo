using UnityEngine;

namespace FpsDemo.Combat
{
    public class DamageHitbox : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private DamageHitZone hitZone = DamageHitZone.Body;

        public DamageHitZone HitZone => hitZone;

        public bool TryGetDamageable(out IDamageable damageable)
        {
            if (health == null)
            {
                health = GetComponentInParent<Health>();
            }

            damageable = health != null
                ? health
                : GetComponentInParent<IDamageable>();

            return damageable != null;
        }
    }
}
