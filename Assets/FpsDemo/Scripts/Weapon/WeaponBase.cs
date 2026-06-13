using System;
using FpsDemo.Combat;
using FpsDemo.Config.Weapon;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public Transform muzzle;
        public abstract WeaponConfig WeaponConfig { get; }
        
        protected Animator Animator;
        protected AudioSource AudioSource;
        public GameObject Owner { get; private set; }
        protected IAimProvider AimProvider;
        protected ICameraRecoilReceiver CameraRecoilReceiver;
        
        [SerializeField]
        protected int currentAmmo;

        public int CurrentAmmo
        {
            get => currentAmmo;
            protected set
            {
                int preAmmo = currentAmmo;
                currentAmmo = Math.Clamp(value, 0, WeaponConfig.MaxAmmo);
                OnAmmoChange?.Invoke(preAmmo, currentAmmo);
            }
        }

        public event Action<int, int> OnAmmoChange;
        
        [SerializeField] private WeaponAltFireBase altFire;
        public bool IsAltFiring => altFire != null && altFire.IsActive;
        
        private void Awake()
        {
            Animator = GetComponent<Animator>();
            AudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            CurrentAmmo = WeaponConfig.MaxAmmo;
        }

        public virtual void StartFire()
        {
            if (Owner == null || AimProvider == null) 
                return;
        }

        public virtual void StopFire()
        {
            if (Owner == null || AimProvider == null) 
                return;
        }
        

        public virtual void Reload()
        {
            if (CurrentAmmo >= WeaponConfig.MaxAmmo) return;

            CurrentAmmo = WeaponConfig.MaxAmmo;
        }

        public void SetOwnerInfo(GameObject go, IAimProvider ap)
        {
            Owner = go;
            AimProvider = ap;
            CameraRecoilReceiver = ap as ICameraRecoilReceiver;

            if (CameraRecoilReceiver == null && go != null)
                CameraRecoilReceiver = go.GetComponent<ICameraRecoilReceiver>();
        }
        
        public virtual void StartAltFire()
        {
            if (Owner == null || AimProvider == null)
                return;

            if (altFire == null)
                return;

            altFire.TryStart(this, new WeaponUseContext(Owner, AimProvider));
        }

        public virtual void StopAltFire()
        {
            if (Owner == null || AimProvider == null)
                return;

            altFire?.Stop(this, new WeaponUseContext(Owner, AimProvider));
        }
        public virtual bool CanAltFire => true;
    }
}
