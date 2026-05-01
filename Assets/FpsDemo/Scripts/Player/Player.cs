using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Player
{
    public class Player : MonoBehaviour
    {
        public PlayerMovement PlayerMovement { get; private set; }
        public PlayerCameraController PlayerCameraController { get; private set; }
        public PlayerInputReader PlayerInputReader  { get; private set; }
    
        public PlayerCombat PlayerCombat { get; private set; }

        private void Awake()
        {
            GameManager.Instance.RegisterPlayer(this);
            PlayerMovement = GetComponent<PlayerMovement>();
            PlayerCameraController = GetComponent<PlayerCameraController>();
            PlayerInputReader = GetComponent<PlayerInputReader>();
            PlayerCombat = GetComponent<PlayerCombat>();
        }

        public void OnDestroy()
        {
            GameManager.Instance.UnregisterPlayer(this);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
