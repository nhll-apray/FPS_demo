using UnityEngine;
using UnityEngine.Serialization;

namespace FpsDemo.Weapon
{
    [CreateAssetMenu(fileName = "HitscanWeaponData", menuName = "Weapon/HitscanWeaponData", order = 0)]
    public class HitscanWeaponData : WeaponData
    {
        [SerializeField] private int damage = 10;
        [SerializeField] private float critDamage = 50f;
        [FormerlySerializedAs("reloadTime")] [SerializeField] private float reloadDuration = 2.5f;
        [SerializeField] private float range = 100f;
        [SerializeField] private float fireInterval = 0.4f;
        [SerializeField] private AudioClip shootSound;
        [SerializeField] private AudioClip reloadSound;
        
        public int Damage => damage;
        public float CritDamage => critDamage;
        public float ReloadDuration => reloadDuration;
        public float Range => range;
        public float FireInterval => fireInterval;
        public AudioClip ShootSound => shootSound;
        public AudioClip ReloadSound => reloadSound;
    }
}