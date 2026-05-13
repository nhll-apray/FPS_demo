using FpsDemo.Combat;
using FpsDemo.Weapon;
using UnityEngine;

namespace FpsDemo.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        private PlayerInputReader _inputReader;
        private IAimProvider _aimProvider;
        private WeaponInventory _weaponInventory;

        private Ray aimRay;
        
        private void Awake()
        {
            _inputReader = GetComponent<PlayerInputReader>();
            _aimProvider = GetComponent<PlayerCameraController>();
            _weaponInventory = GetComponent<WeaponInventory>();
        }

        private void OnEnable()
        {
            if (_inputReader != null)
            {
                _inputReader.OnFireEvent += OnFireInputChanged;
                _inputReader.OnReloadEvent += Reload;
            }
        }

        private void OnDisable()
        {
            if (_inputReader != null)
            {
                _inputReader.OnFireEvent -= OnFireInputChanged;
                _inputReader.OnReloadEvent -= Reload;
            }
        }

        private void Start()
        {
        
        }

        private void Update()
        {
            
        }
    
        private void OnFireInputChanged(bool isFireHeld)
        {
            if (_weaponInventory != null)
            {
                if (isFireHeld)
                {
                    _weaponInventory.StartFire();
                }
                else
                {
                    _weaponInventory.StopFire();
                }
            }
        }
    
        private void Reload()
        {
            if (_weaponInventory != null)
            {
                _weaponInventory.Reload();
            }
        }
    
    }
}
