using UnityEngine;

namespace FpsDemo.Weapon
{
    [CreateAssetMenu(fileName = "HitscanWeaponData", menuName = "Weapon/HitscanWeaponData", order = 0)]
    public class HitscanWeaponData : WeaponData
    {
        [SerializeField]
        private int damage = 10;
        [SerializeField]
        private float critDamage = 50f;
        [SerializeField]
        private float reloadTime = 2.5f;
        [SerializeField]
        private float range = 100f;
        [SerializeField]
        private float fireInterval = 0.4f;
        [SerializeField]
        private AudioClip shootSound;
        
        public int Damage => damage;
        public float CritDamage => critDamage;
        public float ReloadTime => reloadTime;
        public float Range => range;
        public float FireInterval => fireInterval;
        public AudioClip ShootSound => shootSound;
    }
}