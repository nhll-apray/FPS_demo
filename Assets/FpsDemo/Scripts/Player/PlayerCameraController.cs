using FpsDemo.Combat;
using FpsDemo.Config;
using FpsDemo.Config.Player;
using FpsDemo.Game;
using UnityEngine;

namespace FpsDemo.Player
{
    public class PlayerCameraController : MonoBehaviour, IAimProvider, ICameraRecoilReceiver
    {
        public Camera playerCamera;

        private PlayerInputReader _playerInputReader;
        private PlayerCameraEffects _cameraEffects;

        [Header("灵敏度")]
        public float sensitivityX = 1.5f; 
        public float sensitivityY = 1.5f;
    
        private const float MaxLookAngle = 90f;

        private float _cameraPitch = 0f;
        private float _cameraYaw = 0f;
    
        private Vector3 _baseCameraPos;

        public Ray GetAimRay() => playerCamera != null ? playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)) : new Ray();

        private void Awake()
        {
            _playerInputReader = GetComponent<PlayerInputReader>();
            PlayerCameraEffectConfig cameraEffectConfig =
                GameResources.LoadConfig<PlayerCameraEffectConfig>(GameResourcePaths.Config.Player.DefaultCameraEffect);
            _cameraEffects = new PlayerCameraEffects(cameraEffectConfig);
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
            _cameraYaw = transform.localEulerAngles.y;
        
            if (playerCamera != null)
            {
                _baseCameraPos = playerCamera.transform.localPosition;
            }
        }

        private void LateUpdate()
        {
            _cameraEffects?.TickBeforeLook(Time.deltaTime);
            HandleMouseInput();
            _cameraEffects?.TickAfterLook(Time.deltaTime);
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

            if (!Mathf.Approximately(mouseX, 0f) || !Mathf.Approximately(mouseY, 0f))
            {
                Vector2 committedRecoil = _cameraEffects != null ? _cameraEffects.CommitAimRecoil() : Vector2.zero;
                _cameraPitch = Mathf.Clamp(_cameraPitch + committedRecoil.x, -MaxLookAngle, MaxLookAngle);
                _cameraYaw = Mathf.Repeat(_cameraYaw + committedRecoil.y, 360f);
            }
        
            _cameraYaw += mouseX;
            _cameraYaw = Mathf.Repeat(_cameraYaw, 360f);

            _cameraPitch -= mouseY;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -MaxLookAngle, MaxLookAngle);
        }

        //处理相机效果
        private void HandleCameraEffect()
        {
            if (playerCamera == null)
                return;

            Vector2 aimRotationOffset = _cameraEffects != null ? _cameraEffects.AimRotationOffset : Vector2.zero;
            Vector3 effectPositionOffset = _cameraEffects != null ? _cameraEffects.PositionOffset : Vector3.zero;
            Vector3 effectRotationOffset = _cameraEffects != null ? _cameraEffects.RotationOffset : Vector3.zero;
            float pitch = Mathf.Clamp(_cameraPitch + aimRotationOffset.x + effectRotationOffset.x, -MaxLookAngle, MaxLookAngle);

            transform.localRotation = Quaternion.Euler(0f, _cameraYaw + aimRotationOffset.y, 0f);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0f, effectRotationOffset.z);
            playerCamera.transform.localPosition = _baseCameraPos + effectPositionOffset;
        }
        
        public void ApplyCameraRecoil(CameraRecoilSettings recoil)
        {
            _cameraEffects?.ApplyCameraRecoil(recoil);
        }

        public void BeginCameraRecoil()
        {
            _cameraEffects?.BeginCameraRecoil();
        }

        public void EndCameraRecoil()
        {
            _cameraEffects?.EndCameraRecoil();
        }

        public void PlayDamageShake(float intensity)
        {
            _cameraEffects?.PlayDamageShake(intensity);
        }
        
    }
}
