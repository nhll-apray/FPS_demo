using System;
using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public Transform muzzle;
        public abstract WeaponData WeaponData { get; }
        
        protected Animator animator;
        protected AudioSource audioSource;
        public GameObject Owner { get; private set; }
        protected IAimProvider aimProvider;
        protected ICameraRecoilReceiver cameraRecoilReceiver;
        
        [SerializeField]
        protected int currentAmmo;

        public int CurrentAmmo
        {
            get => currentAmmo;
            protected set
            {
                int preAmmo = currentAmmo;
                currentAmmo = Math.Clamp(value, 0, WeaponData.MaxAmmo);
                OnAmmoChange?.Invoke(preAmmo, currentAmmo);
            }
        }

        public event Action<int, int> OnAmmoChange;
        
        [SerializeField] private WeaponAltFireBase altFire;
        public bool IsAltFiring => altFire != null && altFire.IsActive;
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            CurrentAmmo = WeaponData.MaxAmmo;
        }

        public virtual void StartFire()
        {
            if (Owner == null || aimProvider == null) return;
        }

        public virtual void StopFire()
        {
            if (Owner == null || aimProvider == null) return;
        }
        

        public virtual void Reload()
        {
            if (CurrentAmmo >= WeaponData.MaxAmmo) return;

            CurrentAmmo = WeaponData.MaxAmmo;
        }

        public void SetOwnerInfo(GameObject go, IAimProvider ap)
        {
            Owner = go;
            aimProvider = ap;
            cameraRecoilReceiver = ap as ICameraRecoilReceiver;

            if (cameraRecoilReceiver == null && go != null)
                cameraRecoilReceiver = go.GetComponent<ICameraRecoilReceiver>();
        }
        
        public virtual void StartAltFire()
        {
            if (Owner == null || aimProvider == null)
                return;

            if (altFire == null)
                return;

            altFire.TryStart(this, new WeaponUseContext(Owner, aimProvider));
        }

        public virtual void StopAltFire()
        {
            if (Owner == null || aimProvider == null)
                return;

            altFire?.Stop(this, new WeaponUseContext(Owner, aimProvider));
        }
        public virtual bool CanAltFire => true;
    }
}