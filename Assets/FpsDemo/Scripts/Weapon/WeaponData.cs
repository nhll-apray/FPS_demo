using UnityEngine;
using UnityEngine.UI;

namespace FpsDemo.Weapon
{
    public abstract class  WeaponData : ScriptableObject
    {
        [SerializeField] private string weaponName = "DefaultWeaponName";
        public string WeaponName => weaponName;
        [SerializeField] private int maxAmmo;
        public int MaxAmmo => maxAmmo;
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
    }
}