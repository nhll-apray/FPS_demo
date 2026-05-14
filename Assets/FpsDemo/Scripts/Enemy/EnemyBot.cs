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
            _health.died += Die;
        }
        
        private void Die()
        {
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            gameObject.SetActive(false);
            yield return new WaitForSeconds(RespawnDelay);
            transform.position = Vector3.zero;
            _health.ResetHealth();
            gameObject.SetActive(true);
        }
    }
}