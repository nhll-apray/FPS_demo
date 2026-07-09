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

        private readonly GrenadeAltFireRuntime _runtime = new GrenadeAltFireRuntime();
        private GameObject _grenadePrefab;
        private WeaponUseContext _activeContext;
        private GrenadeAltFireConfig _activeConfig;

        public float CooldownRemaining => _runtime.GetCooldownRemaining(Time.time);
        public float CooldownNormalized => _runtime.GetCooldownNormalized(Time.time);
        public bool IsCoolingDown => _runtime.IsCoolingDown(Time.time);
        public bool IsReady => !IsActive && !IsCoolingDown;

        private void Awake()
        {
            _runtime.ReleaseRequested += OnRuntimeReleaseRequested;
            _runtime.Finished += OnRuntimeFinished;
        }

        private void Update()
        {
            _runtime.Tick(Time.time);
        }

        protected override bool CanStart(WeaponBase weapon, WeaponUseContext context)
        {
            GrenadeAltFireConfig grenadeData = GetConfig();
            return grenadeData != null
                   && GetGrenadePrefab(grenadeData) != null
                   && _runtime.CanStart(Time.time)
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

            _activeContext = context;
            _activeConfig = grenadeData;
            bool started = _runtime.TryStart(Time.time, CreateRuntimeSettings(grenadeData));
            if (!started)
            {
                Finish();
            }
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

        private static GrenadeAltFireRuntimeSettings CreateRuntimeSettings(GrenadeAltFireConfig grenadeConfig)
        {
            return new GrenadeAltFireRuntimeSettings(
                grenadeConfig.releaseDelay,
                grenadeConfig.finishDelay,
                grenadeConfig.cooldown);
        }

        private void OnRuntimeReleaseRequested()
        {
            SpawnGrenade(_activeContext, _activeConfig);
            NotifyReleased();
        }

        private void OnRuntimeFinished()
        {
            _activeContext = default;
            _activeConfig = null;
            Finish();
        }
    }
}
