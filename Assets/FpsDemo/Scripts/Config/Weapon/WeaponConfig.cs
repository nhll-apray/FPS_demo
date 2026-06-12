using UnityEngine;

namespace FpsDemo.Config.Weapon
{
    public abstract class WeaponConfig : ScriptableObject
    {
        [SerializeField] private string weaponName = "DefaultWeaponName";
        public string WeaponName => weaponName;

        [SerializeField] private int maxAmmo;
        public int MaxAmmo => maxAmmo;

        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
    }
}
