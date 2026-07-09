using System;
using UnityEngine;

namespace FpsDemo.Config.Level
{
    [Serializable]
    public class LevelWaveConfig
    {
        [SerializeField] private string waveName = "Wave";
        [SerializeField, Min(0f)] private float delayBeforeWave = 1f;
        [SerializeField] private LevelSpawnGroupConfig[] spawnGroups;

        public string WaveName => waveName;
        public float DelayBeforeWave => Mathf.Max(0f, delayBeforeWave);
        public LevelSpawnGroupConfig[] SpawnGroups => spawnGroups;
    }
}
