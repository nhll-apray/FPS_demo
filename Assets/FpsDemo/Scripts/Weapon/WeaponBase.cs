using System;
using UnityEngine;

namespace FpsDemo.Weapon
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public Transform muzzle;
        public WeaponData weaponData;
        
        protected Animator animator;
        protected AudioSource audioSource;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        protected virtual void StartFire()
        {
            
        }

        protected virtual void StopFire()
        {
            
        }

        public void Reload()
        {
            
        }
    }
}