using System.Collections;
using UnityEngine;

namespace FpsDemo.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float respawnDelay = 2.5f;
        
        [SerializeField] private GameObject currentEnemy;

        private Coroutine _respawnCoroutine;

        private void Start()
        {
            if (spawnPoint == null)
                spawnPoint = transform;

            if (currentEnemy == null)
            {
                SpawnEnemy();
            }
        }

        private void Update()
        {
            if (currentEnemy == null && _respawnCoroutine == null)
            {
                _respawnCoroutine = StartCoroutine(RespawnRoutine());
            }
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);

            SpawnEnemy();

            _respawnCoroutine = null;
        }

        private void SpawnEnemy()
        {
            if (enemyPrefab == null)
                return;

            currentEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}