using FpsDemo.Game;
using FpsDemo.Weapon;
using UnityEngine;

namespace FpsDemo.Player
{
    public class PlayerEntity : MonoBehaviour
    {
        public PlayerMovement PlayerMovement { get; private set; }
        public PlayerCameraController PlayerCameraController { get; private set; }
        public PlayerInputReader PlayerInputReader  { get; private set; }
    
        public PlayerCombat PlayerCombat { get; private set; }
        
        public WeaponInventory WeaponInventory { get; private set; }

        private void Awake()
        {
            GameManager.Instance.RegisterPlayer(this);
            PlayerMovement = GetComponent<PlayerMovement>();
            PlayerCameraController = GetComponent<PlayerCameraController>();
            PlayerInputReader = GetComponent<PlayerInputReader>();
            PlayerCombat = GetComponent<PlayerCombat>();
            WeaponInventory = GetComponent<WeaponInventory>();
        }

        public void OnDestroy()
        {
            GameManager.Instance.UnregisterPlayer(this);
        }
    }
}
