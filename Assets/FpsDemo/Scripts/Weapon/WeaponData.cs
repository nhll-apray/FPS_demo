using UnityEngine;

namespace FpsDemo.Weapon
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/WeaponData", order = 0)]
    public class WeaponData : ScriptableObject
    {
        public string weaponName = "AK47";

        public float damage = 10f;
        public float critDamage = 50f;
        public float fireRate = 1.0f;
        public float reloadTime = 2.5f;
        public int maxAmmo = 200;
        public float range = 100f;
    }
}