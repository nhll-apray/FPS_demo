using System.Collections;
using FpsDemo.Combat;
using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public class GrenadeAltFire : WeaponAltFireBase
    {
        [SerializeField] private Transform throwOrigin;

        private float _nextReadyTime;

        protected override bool CanStart(WeaponBase weapon, WeaponUseContext context)
        {
            GrenadeAltFireData grenadeData = GetData();
            return grenadeData != null
                   && grenadeData.GrenadePrefab != null
                   && Time.time >= _nextReadyTime
                   && context.aimProvider != null
                   && context.owner != null;
        }

        protected override void OnStart(WeaponBase weapon, WeaponUseContext context)
        {
            GrenadeAltFireData grenadeData = GetData();
            if (grenadeData == null)
            {
                Finish();
                return;
            }

            StartCoroutine(ThrowRoutine(context, grenadeData));
        }

        private IEnumerator ThrowRoutine(WeaponUseContext context, GrenadeAltFireData grenadeData)
        {
            yield return new WaitForSeconds(grenadeData.releaseDelay);

            SpawnGrenade(context, grenadeData);
            NotifyReleased();

            yield return new WaitForSeconds(Mathf.Max(0f, grenadeData.finishDelay - grenadeData.releaseDelay));

            _nextReadyTime = Time.time + grenadeData.cooldown;
            Finish();
        }

        private void SpawnGrenade(WeaponUseContext context, GrenadeAltFireData grenadeData)
        {
            Ray aimRay = context.aimProvider.GetAimRay();
            Vector3 origin = throwOrigin != null ? throwOrigin.position : aimRay.origin;

            GameObject grenadePrefab = grenadeData.GrenadePrefab;
            if (grenadePrefab == null)
                return;

            GameObject grenade = Instantiate(grenadePrefab, origin, Quaternion.LookRotation(aimRay.direction));
            GrenadeProjectile projectile = grenade.GetComponent<GrenadeProjectile>();

            if (projectile != null)
            {
                projectile.Launch(context.owner, aimRay.direction, grenadeData);
            }
        }

        private GrenadeAltFireData GetData()
        {
            return GameResources.LoadData<GrenadeAltFireData>(GameResourcePaths.Data.Weapon.GrenadeAltFireData);
        }
    }
}
