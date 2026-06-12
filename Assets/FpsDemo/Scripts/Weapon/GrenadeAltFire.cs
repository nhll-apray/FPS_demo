using System.Collections;
using FpsDemo.Combat;
using FpsDemo.Config;
using FpsDemo.Config.Weapon;
using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public class GrenadeAltFire : WeaponAltFireBase
    {
        [SerializeField] private Transform throwOrigin;

        private float _nextReadyTime;
        private GameObject _grenadePrefab;

        protected override bool CanStart(WeaponBase weapon, WeaponUseContext context)
        {
            GrenadeAltFireConfig grenadeData = GetConfig();
            return grenadeData != null
                   && GetGrenadePrefab(grenadeData) != null
                   && Time.time >= _nextReadyTime
                   && context.aimProvider != null
                   && context.owner != null;
        }

        protected override void OnStart(WeaponBase weapon, WeaponUseContext context)
        {
            GrenadeAltFireConfig grenadeData = GetConfig();
            if (grenadeData == null)
            {
                Finish();
                return;
            }

            StartCoroutine(ThrowRoutine(context, grenadeData));
        }

        private IEnumerator ThrowRoutine(WeaponUseContext context, GrenadeAltFireConfig grenadeData)
        {
            yield return new WaitForSeconds(grenadeData.releaseDelay);

            SpawnGrenade(context, grenadeData);
            NotifyReleased();

            yield return new WaitForSeconds(Mathf.Max(0f, grenadeData.finishDelay - grenadeData.releaseDelay));

            _nextReadyTime = Time.time + grenadeData.cooldown;
            Finish();
        }

        private void SpawnGrenade(WeaponUseContext context, GrenadeAltFireConfig grenadeData)
        {
            Ray aimRay = context.aimProvider.GetAimRay();
            Vector3 origin = throwOrigin != null ? throwOrigin.position : aimRay.origin;

            GameObject grenadePrefab = GetGrenadePrefab(grenadeData);
            if (grenadePrefab == null)
                return;

            GameObject grenade = Instantiate(grenadePrefab, origin, Quaternion.LookRotation(aimRay.direction));
            GrenadeProjectile projectile = grenade.GetComponent<GrenadeProjectile>();

            if (projectile != null)
            {
                projectile.Launch(context.owner, aimRay.direction, grenadeData);
            }
        }

        private GrenadeAltFireConfig GetConfig()
        {
            return GameResources.LoadConfig<GrenadeAltFireConfig>(GameResourcePaths.Config.Weapon.GrenadeAltFire);
        }

        private GameObject GetGrenadePrefab(GrenadeAltFireConfig grenadeConfig)
        {
            if (grenadeConfig == null)
            {
                return null;
            }

            return _grenadePrefab != null
                ? _grenadePrefab
                : _grenadePrefab = GameResources.LoadPrefab(grenadeConfig.GrenadePrefabPath);
        }
    }
}
