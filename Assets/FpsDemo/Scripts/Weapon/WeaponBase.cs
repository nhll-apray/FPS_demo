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
        protected GameObject owner;
        protected IAimProvider aimProvider;
        
        [SerializeField]
        protected int currentAmmo;
        public int CurrentAmmo { get => currentAmmo; protected set => currentAmmo = value; }

        
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
            if (owner == null || aimProvider == null) return;
            Debug.Log("StartFire");
        }

        public virtual void StopFire()
        {
            if (owner == null || aimProvider == null) return;
            Debug.Log("StopFire");
        }
        

        public virtual void Reload()
        {
            Debug.Log("Reload");
            if (currentAmmo >= WeaponData.MaxAmmo) return;

            currentAmmo = WeaponData.MaxAmmo;
        }

        public void SetOwnerInfo(GameObject go, IAimProvider ap)
        {
            owner = go;
            aimProvider = ap;
        }
    }
}