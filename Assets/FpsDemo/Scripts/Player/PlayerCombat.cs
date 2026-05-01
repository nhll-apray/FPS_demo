using UnityEngine;

namespace FpsDemo.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        private PlayerInputReader _inputReader;
        private Camera _camera;
        
        private void Awake()
        {
            _inputReader = GetComponent<PlayerInputReader>();
            _camera = GetComponent<PlayerCameraController>().playerCamera;
        }

        private void OnEnable()
        {
            if (_inputReader != null)
            {
                _inputReader.OnFireEvent += Fire;
                _inputReader.OnReloadEvent += Reload;
            }
        }

        private void OnDisable()
        {
            if (_inputReader != null)
            {
                _inputReader.OnFireEvent -= Fire;
                _inputReader.OnReloadEvent -= Reload;
            }
        }

        private void Start()
        {
        
        }

        private void Update()
        {
        
        }
    
        private void Fire(bool isPressed)
        {

        }
    
        private void Reload()
        {
        
        }
    
    }
}
