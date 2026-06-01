using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Weapon
{
    [CreateAssetMenu(fileName = "GrenadeAltFireData", menuName = "Weapon/AltFire/Grenade")]
    public class GrenadeAltFireData : ScriptableObject
    {
        public int damage = 80;
        public float radius = 5f;
        public float fuseTime = 2f;
        public float throwForce = 16f;
        public float upwardForce = 3f;
        public bool applyExplosionForce = true;
        public float explosionForce = 850f;
        public float explosionUpwardsModifier = 3f;
        public float cooldown = 1.2f;
        public float releaseDelay = 0.35f;
        public float finishDelay = 0.8f;
        public LayerMask damageMask;

        public GameObject GrenadePrefab => GameResources.LoadPrefab(GameResourcePaths.Prefabs.Projectiles.HandGrenade);
        public GameObject ExplosionPrefab => GameResources.LoadPrefab(GameResourcePaths.Prefabs.VFX.Explosion);

        private void OnValidate()
        {
            if (damageMask.value == 0)
            {
                damageMask = LayerMask.GetMask("Enemy");
            }
        }
    }
}
