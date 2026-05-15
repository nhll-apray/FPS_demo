using System;
using FpsDemo.Game;
using FpsDemo.Weapon;
using FpsDemo.Player;
using UnityEngine;

namespace FpsDemo.UI
{
    public class PlayerHUD : MonoBehaviour
    {
        private PlayerEntity _player;
        private WeaponInventory _weaponInventory;

        [SerializeField] private WeaponHUD weaponHUD;
        
        private void Start()
        {
            _player = GameManager.Instance.CurrentPlayer;
            _weaponInventory = _player.WeaponInventory;
            Bind();
        }

        private void Bind()
        {
            if (weaponHUD != null)
            {
                weaponHUD.Bind(_weaponInventory);
            }
        }
    }
}