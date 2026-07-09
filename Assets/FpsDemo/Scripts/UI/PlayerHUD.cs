using FpsDemo.Combat;
using FpsDemo.Game;
using FpsDemo.Player;
using FpsDemo.Weapon;
using UnityEngine;

namespace FpsDemo.UI
{
    public class PlayerHUD : MonoBehaviour
    {
        private PlayerEntity _player;
        private WeaponInventory _weaponInventory;
        private Health _health;

        [SerializeField] private WeaponHUD weaponHUD;
        [SerializeField] private GrenadeCooldownHUD grenadeCooldownHUD;
        [SerializeField] private PlayerHealthHUD healthHUD;
        [SerializeField] private PlayerDamageOverlayHUD damageOverlayHUD;

        private void Start()
        {
            _player = GameManager.Instance != null ? GameManager.Instance.CurrentPlayer : null;
            if (_player == null)
            {
                return;
            }

            _weaponInventory = _player.WeaponInventory;
            _health = _player.GetComponent<Health>();
            Bind();
        }

        private void Bind()
        {
            if (weaponHUD != null)
            {
                weaponHUD.Bind(_weaponInventory);
            }

            if (grenadeCooldownHUD != null)
            {
                grenadeCooldownHUD.Bind(_weaponInventory);
            }

            if (healthHUD != null)
            {
                healthHUD.Bind(_health);
            }

            if (damageOverlayHUD != null)
            {
                damageOverlayHUD.Bind(_player);
            }
        }
    }
}
