using UnityEngine;

namespace FpsDemo.Weapon
{
    public class WeaponInventory : MonoBehaviour
    {
        [field: SerializeField]
        public WeaponBase CurrentWeapon { get; private set; }
        
    }
}