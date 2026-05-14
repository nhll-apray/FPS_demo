using System;
using System.Collections;
using FpsDemo.Combat;
using UnityEngine;

namespace FpsDemo.Enemy
{
    public class EnemyBot : MonoBehaviour
    {
        private Health _health;
        private const float RespawnDelay = 2f;


        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnDied += Die;
        }
        
        private void Die()
        {
            Destroy(gameObject);
        }
        
    }
}