using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FpsDemo.Player
{
    public class PlayerInputReader : MonoBehaviour, PlayerControls.IPlayerActions
    {
        public static PlayerInputReader Instance { get; private set; }
    
        private PlayerControls _controls;
    
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }

        public event Action OnWeapon1Event;
        public event Action OnWeapon2Event;
        public event Action OnReloadEvent;
    
        public event Action<bool> OnJumpEvent;
        public event Action<bool> OnShiftEvent;
        public event Action<bool> OnFireEvent;
        public event Action<bool> OnAltFireEvent;
        public event Action<bool> OnInteractEvent;
        public event Action<bool> OnCrouchEvent;
        public event Action<bool> OnUltEvent;
        public event Action<bool> OnSkillEvent;

        private void Awake()
        {
            _controls = new PlayerControls();
            _controls.Player.SetCallbacks(this);
        }

        private void OnEnable() => _controls.Player.Enable();
        private void OnDisable() => _controls.Player.Disable();

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnFireEvent?.Invoke(true);
            if (context.phase == InputActionPhase.Canceled) OnFireEvent?.Invoke(false);
        }

        public void OnAltFire(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnAltFireEvent?.Invoke(true);
            if (context.phase == InputActionPhase.Canceled) OnAltFireEvent?.Invoke(false);
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnInteractEvent?.Invoke(true);
            if (context.phase == InputActionPhase.Canceled) OnInteractEvent?.Invoke(false);
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnCrouchEvent?.Invoke(true);
            if (context.phase == InputActionPhase.Canceled) OnCrouchEvent?.Invoke(false);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnJumpEvent?.Invoke(true);
            if (context.phase == InputActionPhase.Canceled) OnJumpEvent?.Invoke(false);
        }

        public void OnWeapon1(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnWeapon1Event?.Invoke();
        }

        public void OnWeapon2(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnWeapon2Event?.Invoke();
        }

        public void OnShift(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnShiftEvent?.Invoke(true);
            if (context.phase == InputActionPhase.Canceled) OnShiftEvent?.Invoke(false);
        }

        public void OnSkill(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnSkillEvent?.Invoke(true);
            if (context.phase == InputActionPhase.Canceled) OnSkillEvent?.Invoke(false);
        }

        public void OnUlt(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnUltEvent?.Invoke(true);
            if (context.phase == InputActionPhase.Canceled) OnUltEvent?.Invoke(false);
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) OnReloadEvent?.Invoke();
        }
    }
}