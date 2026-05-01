using UnityEngine;

namespace FpsDemo.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
    
        public Player.Player CurrentPlayer { get; private set; }

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void RegisterPlayer(Player.Player player)
        {
            if (CurrentPlayer == null)
            {
                CurrentPlayer = player;
            }
        }
    
        public void UnregisterPlayer(Player.Player player)
        {
            if (CurrentPlayer == player)
            {
                CurrentPlayer = null;
            }
        }
    }
}
