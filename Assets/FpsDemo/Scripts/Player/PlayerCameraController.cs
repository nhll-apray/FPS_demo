using UnityEngine;

namespace FpsDemo.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        public Camera playerCamera;

        [SerializeField]
        private PlayerCameraEffectProfile cameraEffectProfile;
    
        private PlayerInputReader _playerInputReader;
        private PlayerCameraEffects _cameraEffects;

        [Header("灵敏度")]
        public float sensitivityX = 1.5f; 
        public float sensitivityY = 1.5f;
    
        private const float MaxLookAngle = 90f;
    

        private float _cameraPitch = 0f;
    
        private Vector3 _baseCameraPos;

        private void Awake()
        {
            _playerInputReader = GetComponent<PlayerInputReader>();
            _cameraEffects = new PlayerCameraEffects(cameraEffectProfile);
        }

        private void OnEnable()
        {
            _cameraEffects?.Enable();
        }

        private void OnDisable()
        {
            _cameraEffects?.Disable();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        
            if (playerCamera != null)
            {
                _baseCameraPos = playerCamera.transform.localPosition;
            }
        }

        private void LateUpdate()
        {
            HandleMouseInput();
            Vector2 moveInput = _playerInputReader != null ? _playerInputReader.MoveInput : Vector2.zero;
            _cameraEffects?.Tick(Time.deltaTime, moveInput);
            HandleCameraEffect();
        }

        //视角控制
        private void HandleMouseInput()
        {
            if (_playerInputReader == null || playerCamera == null)
                return;

            Vector2 lookInput = _playerInputReader.LookInput;

            float mouseX = lookInput.x * sensitivityX * 0.1f; 
            float mouseY = lookInput.y * sensitivityY * 0.1f;
        
            transform.Rotate(Vector3.up * mouseX);
        
            _cameraPitch -= mouseY;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -MaxLookAngle, MaxLookAngle);
        }

        //处理相机效果
        private void HandleCameraEffect()
        {
            if (playerCamera == null)
                return;

            Vector3 effectPositionOffset = _cameraEffects != null ? _cameraEffects.PositionOffset : Vector3.zero;
            Vector3 effectRotationOffset = _cameraEffects != null ? _cameraEffects.RotationOffset : Vector3.zero;
            playerCamera.transform.localEulerAngles = new Vector3(_cameraPitch + effectRotationOffset.x, 0f, effectRotationOffset.z);
            playerCamera.transform.localPosition = _baseCameraPos + effectPositionOffset;
        }
    }
}