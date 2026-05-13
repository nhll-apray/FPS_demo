using UnityEngine;

namespace FpsDemo.Combat
{
    public struct WeaponUseContext
    {
        public readonly IAimProvider aimProvider;
        public readonly GameObject owner;
            
        public WeaponUseContext(GameObject gameObject, IAimProvider aimProvider)
        {
            this.aimProvider = aimProvider;
            owner = gameObject;
        }
    }
}