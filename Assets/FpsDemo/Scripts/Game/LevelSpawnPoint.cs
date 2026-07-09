using UnityEngine;

namespace FpsDemo.Game
{
    public class LevelSpawnPoint : MonoBehaviour
    {
        [SerializeField] private string spawnPointId = "spawn_a";

        public string SpawnPointId => spawnPointId;
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        public Vector3 Right => transform.right;
    }
}
