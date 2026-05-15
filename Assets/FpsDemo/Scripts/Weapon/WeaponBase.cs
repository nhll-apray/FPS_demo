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
            Debug.Log("StartFire");
        }

        public virtual void StopFire()
        {
            if (Owner == null || aimProvider == null) return;
            Debug.Log("StopFire");
        }
        

        public virtual void Reload()
        {
            Debug.Log("Reload");
            if (CurrentAmmo >= WeaponData.MaxAmmo) return;

            CurrentAmmo = WeaponData.MaxAmmo;
        }

        public void SetOwnerInfo(GameObject go, IAimProvider ap)
        {
            Owner = go;
            aimProvider = ap;
        }
    }
}