using System;
using System.Collections;
using System.Collections.Generic;
using FpsDemo.Combat;
using FpsDemo.Config;
using FpsDemo.Config.Enemy;
using FpsDemo.Config.Level;
using FpsDemo.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FpsDemo.Game
{
    public class LevelDirector : MonoBehaviour
    {
        public enum LevelState
        {
            Ready,
            Playing,
            Paused,
            Victory,
            Defeat
        }

        [Header("Level")]
        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private string levelConfigPath = GameResourcePaths.Config.Level.Level01;
        [SerializeField] private Health playerHealth;

        [Header("Flow")]
        [SerializeField] private bool autoStartOnLoad;

        [Header("Input")]
        [SerializeField] private Key pauseKey = Key.Escape;

        [Header("Cleanup")]
        [SerializeField] private bool despawnExistingEnemiesOnStart = true;

        private readonly Dictionary<Health, Action> _enemyDeathHandlers = new Dictionary<Health, Action>();
        private readonly Dictionary<string, LevelSpawnPoint> _spawnPointsById =
            new Dictionary<string, LevelSpawnPoint>(StringComparer.OrdinalIgnoreCase);
        private readonly List<GameObject> _spawnedEnemies = new List<GameObject>();

        private bool _isListeningToPlayerDeath;
        private bool _usesWavesThisRun;
        private bool _isWaitingForNextWave;
        private int _currentWaveIndex = -1;
        private Coroutine _waveRoutine;
        private float _levelStartTime;
        private float _levelFinishTime;
        private float _timeScaleBeforePause = 1f;
        private bool _isTimeScalePaused;

        public LevelState State { get; private set; } = LevelState.Ready;
        public int AliveEnemyCount { get; private set; }
        public int KilledEnemyCount { get; private set; }
        public int TotalEnemyCount { get; private set; }
        public int CurrentWaveAliveEnemyCount { get; private set; }
        public int CurrentWaveKilledEnemyCount { get; private set; }
        public int CurrentWaveTotalEnemyCount { get; private set; }
        public LevelResult CurrentResult { get; private set; } = LevelResult.Empty;
        public bool AutoStartOnLoad => autoStartOnLoad;
        public bool IsUsingWaves => HasConfiguredWaves();
        public bool IsWaitingForNextWave => _isWaitingForNextWave;
        public int CurrentWaveNumber => HasConfiguredWaves() ? Mathf.Clamp(_currentWaveIndex + 1, 1, TotalWaveCount) : 0;
        public int TotalWaveCount => levelConfig != null && levelConfig.Waves != null ? levelConfig.Waves.Length : 0;
        public string CurrentWaveName
        {
            get
            {
                LevelWaveConfig wave = GetWave(_currentWaveIndex);
                return wave != null ? wave.WaveName : string.Empty;
            }
        }
        public float ElapsedTime => State == LevelState.Playing || State == LevelState.Paused
            ? Time.time - _levelStartTime
            : Mathf.Max(0f, _levelFinishTime - _levelStartTime);

        public event Action<LevelState> OnStateChanged;

        private void Start()
        {
            ResolveSceneReferences();
            EnterReady();

            if (autoStartOnLoad)
            {
                BeginLevel();
            }
        }

        private void Update()
        {
            if (WasPauseKeyPressed())
            {
                TogglePause();
            }

        }

        private void OnDisable()
        {
            RestoreTimeScale();
            StopWaveRoutine();
            UnsubscribePlayerDeath();
            ClearEnemyDeathHandlers();
        }

        public void BeginLevel()
        {
            if (State == LevelState.Playing || State == LevelState.Paused)
            {
                return;
            }

            ResolveSceneReferences();
            if (!CanStartLevel())
            {
                return;
            }

            CleanupBeforeStart();
            StopWaveRoutine();
            RestoreTimeScale();

            _usesWavesThisRun = HasConfiguredWaves();
            _currentWaveIndex = -1;
            _isWaitingForNextWave = false;
            AliveEnemyCount = 0;
            KilledEnemyCount = 0;
            TotalEnemyCount = CalculatePlannedEnemyCount();
            CurrentWaveAliveEnemyCount = 0;
            CurrentWaveKilledEnemyCount = 0;
            CurrentWaveTotalEnemyCount = 0;
            CurrentResult = LevelResult.Empty;
            _levelStartTime = Time.time;
            _levelFinishTime = _levelStartTime;
            SetState(LevelState.Playing);

            SubscribePlayerDeath();
            StartWave(0);
        }

        public void TogglePause()
        {
            if (State == LevelState.Playing)
            {
                PauseLevel();
                return;
            }

            if (State == LevelState.Paused)
            {
                ResumeLevel();
            }
        }

        public void PauseLevel()
        {
            if (State != LevelState.Playing)
            {
                return;
            }

            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            _isTimeScalePaused = true;
            SetState(LevelState.Paused);
        }

        public void ResumeLevel()
        {
            if (State != LevelState.Paused)
            {
                return;
            }

            RestoreTimeScale();
            SetState(LevelState.Playing);
        }

        public void EnterReady()
        {
            if (State == LevelState.Playing)
            {
                return;
            }

            RestoreTimeScale();
            AliveEnemyCount = 0;
            KilledEnemyCount = 0;
            TotalEnemyCount = 0;
            CurrentWaveAliveEnemyCount = 0;
            CurrentWaveKilledEnemyCount = 0;
            CurrentWaveTotalEnemyCount = 0;
            _usesWavesThisRun = false;
            _currentWaveIndex = -1;
            _isWaitingForNextWave = false;
            CurrentResult = LevelResult.Empty;
            _levelStartTime = 0f;
            _levelFinishTime = 0f;
            SetState(LevelState.Ready);
        }

        private void ResolveSceneReferences()
        {
            ResolveLevelConfig();
            CacheSpawnPoints();

            if (playerHealth != null)
            {
                return;
            }

            PlayerEntity player = GameManager.Instance != null
                ? GameManager.Instance.CurrentPlayer
                : null;

            if (player == null)
            {
                player = FindFirstObjectByType<PlayerEntity>();
            }

            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
            }
        }

        private void ResolveLevelConfig()
        {
            if (levelConfig != null && !LevelSelectionContext.HasSelectedLevel)
            {
                return;
            }

            string selectedLevelConfigPath = LevelSelectionContext.GetSelectedOrDefault(levelConfigPath);
            levelConfig = GameResources.LoadConfig<LevelConfig>(selectedLevelConfigPath);
        }

        private void CacheSpawnPoints()
        {
            _spawnPointsById.Clear();
            LevelSpawnPoint[] spawnPoints = FindObjectsByType<LevelSpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                LevelSpawnPoint spawnPoint = spawnPoints[i];
                if (spawnPoint == null || string.IsNullOrWhiteSpace(spawnPoint.SpawnPointId))
                {
                    continue;
                }

                if (_spawnPointsById.ContainsKey(spawnPoint.SpawnPointId))
                {
                    continue;
                }

                _spawnPointsById.Add(spawnPoint.SpawnPointId, spawnPoint);
            }
        }

        private bool CanStartLevel()
        {
            if (!HasConfiguredWaves())
            {
                return false;
            }

            if (playerHealth == null)
            {
                return false;
            }

            return true;
        }

        private void CleanupBeforeStart()
        {
            UnsubscribePlayerDeath();
            ClearEnemyDeathHandlers();

            if (despawnExistingEnemiesOnStart)
            {
                DespawnTrackedEnemies();
            }
        }

        private void DespawnTrackedEnemies()
        {
            for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
            {
                if (_spawnedEnemies[i] != null)
                {
                    Destroy(_spawnedEnemies[i]);
                }
            }

            _spawnedEnemies.Clear();
        }

        private void TrackEnemy(GameObject enemy)
        {
            if (enemy == null)
            {
                return;
            }

            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth == null)
            {
                return;
            }

            if (enemyHealth.IsDead || _enemyDeathHandlers.ContainsKey(enemyHealth))
            {
                return;
            }

            Action deathHandler = () => HandleEnemyDied(enemyHealth);
            _enemyDeathHandlers.Add(enemyHealth, deathHandler);
            enemyHealth.OnDied += deathHandler;
            _spawnedEnemies.Add(enemy);
            AliveEnemyCount++;
            CurrentWaveAliveEnemyCount++;
            CurrentWaveTotalEnemyCount++;
        }

        private void HandleEnemyDied(Health enemyHealth)
        {
            if (State != LevelState.Playing)
            {
                return;
            }

            UntrackEnemy(enemyHealth);
            KilledEnemyCount++;
            AliveEnemyCount = Mathf.Max(0, AliveEnemyCount - 1);
            CurrentWaveKilledEnemyCount++;
            CurrentWaveAliveEnemyCount = Mathf.Max(0, CurrentWaveAliveEnemyCount - 1);

            if (AliveEnemyCount <= 0)
            {
                TryAdvanceWaveOrFinish();
            }
        }

        private void SubscribePlayerDeath()
        {
            if (playerHealth == null || _isListeningToPlayerDeath)
            {
                return;
            }

            playerHealth.OnDied += HandlePlayerDied;
            _isListeningToPlayerDeath = true;
        }

        private void UnsubscribePlayerDeath()
        {
            if (playerHealth == null || !_isListeningToPlayerDeath)
            {
                return;
            }

            playerHealth.OnDied -= HandlePlayerDied;
            _isListeningToPlayerDeath = false;
        }

        private void HandlePlayerDied()
        {
            if (State == LevelState.Playing)
            {
                FinishLevel(LevelState.Defeat);
            }
        }

        private void FinishLevel(LevelState resultState)
        {
            if (State != LevelState.Playing && State != LevelState.Paused)
            {
                return;
            }

            RestoreTimeScale();
            _levelFinishTime = Time.time;
            _isWaitingForNextWave = false;
            CurrentResult = LevelScoreCalculator.CreateResult(
                ToLevelResultType(resultState),
                KilledEnemyCount,
                TotalEnemyCount,
                Mathf.Max(0f, _levelFinishTime - _levelStartTime));
            SetState(resultState);
            UnsubscribePlayerDeath();
            ClearEnemyDeathHandlers();
        }

        private static LevelResultType ToLevelResultType(LevelState state)
        {
            return state == LevelState.Victory ? LevelResultType.Victory : LevelResultType.Defeat;
        }

        private void RestoreTimeScale()
        {
            if (!_isTimeScalePaused)
            {
                return;
            }

            Time.timeScale = Mathf.Approximately(_timeScaleBeforePause, 0f)
                ? 1f
                : _timeScaleBeforePause;
            _isTimeScalePaused = false;
        }

        private void StartWave(int waveIndex)
        {
            StopWaveRoutine();
            _waveRoutine = StartCoroutine(StartWaveRoutine(waveIndex));
        }

        private IEnumerator StartWaveRoutine(int waveIndex)
        {
            if (!HasConfiguredWaves() || waveIndex < 0 || waveIndex >= TotalWaveCount)
            {
                yield break;
            }

            _currentWaveIndex = waveIndex;
            CurrentWaveAliveEnemyCount = 0;
            CurrentWaveKilledEnemyCount = 0;
            CurrentWaveTotalEnemyCount = 0;
            _isWaitingForNextWave = true;

            LevelWaveConfig wave = GetWave(waveIndex);
            float delay = wave != null ? wave.DelayBeforeWave : 0f;
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            if (State != LevelState.Playing)
            {
                _waveRoutine = null;
                yield break;
            }

            _isWaitingForNextWave = false;
            SpawnWaveEnemies(wave);

            bool shouldAdvanceImmediately = CurrentWaveAliveEnemyCount <= 0;
            _waveRoutine = null;

            if (shouldAdvanceImmediately)
            {
                TryAdvanceWaveOrFinish();
            }
        }

        private void SpawnWaveEnemies(LevelWaveConfig wave)
        {
            if (wave == null || wave.SpawnGroups == null)
            {
                return;
            }

            for (int i = 0; i < wave.SpawnGroups.Length; i++)
            {
                SpawnGroup(wave.SpawnGroups[i]);
            }
        }

        private void SpawnGroup(LevelSpawnGroupConfig group)
        {
            if (group == null)
            {
                return;
            }

            if (!TryGetSpawnPoint(group.SpawnPointId, out LevelSpawnPoint spawnPoint))
            {
                return;
            }

            GameObject prefab = GameResources.LoadPrefab(group.EnemyType.GetPrefabPath());
            if (prefab == null)
            {
                return;
            }

            int count = group.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject enemy = Instantiate(prefab, GetSpawnPosition(spawnPoint, i, count, group.Spacing), spawnPoint.Rotation);
                TrackEnemy(enemy);
            }
        }

        private bool TryGetSpawnPoint(string spawnPointId, out LevelSpawnPoint spawnPoint)
        {
            if (string.IsNullOrWhiteSpace(spawnPointId))
            {
                spawnPoint = null;
                return false;
            }

            if (_spawnPointsById.Count == 0)
            {
                CacheSpawnPoints();
            }

            if (_spawnPointsById.TryGetValue(spawnPointId, out spawnPoint))
            {
                return true;
            }

            return false;
        }

        private static Vector3 GetSpawnPosition(LevelSpawnPoint spawnPoint, int index, int count, float spacing)
        {
            if (count <= 1)
            {
                return spawnPoint.Position;
            }

            float centerOffset = (count - 1) * 0.5f;
            float offset = (index - centerOffset) * spacing;
            return spawnPoint.Position + spawnPoint.Right * offset;
        }

        private void TryAdvanceWaveOrFinish()
        {
            if (_usesWavesThisRun && _currentWaveIndex + 1 < TotalWaveCount)
            {
                StartWave(_currentWaveIndex + 1);
                return;
            }

            FinishLevel(LevelState.Victory);
        }

        private int CalculatePlannedEnemyCount()
        {
            if (!HasConfiguredWaves())
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < levelConfig.Waves.Length; i++)
            {
                LevelWaveConfig wave = levelConfig.Waves[i];
                if (wave == null || wave.SpawnGroups == null)
                {
                    continue;
                }

                for (int j = 0; j < wave.SpawnGroups.Length; j++)
                {
                    if (wave.SpawnGroups[j] != null)
                    {
                        count += wave.SpawnGroups[j].Count;
                    }
                }
            }

            return count;
        }

        private bool HasConfiguredWaves()
        {
            return levelConfig != null && levelConfig.Waves != null && levelConfig.Waves.Length > 0;
        }

        private LevelWaveConfig GetWave(int waveIndex)
        {
            if (levelConfig == null || levelConfig.Waves == null || waveIndex < 0 || waveIndex >= levelConfig.Waves.Length)
            {
                return null;
            }

            return levelConfig.Waves[waveIndex];
        }

        private void StopWaveRoutine()
        {
            if (_waveRoutine != null)
            {
                StopCoroutine(_waveRoutine);
                _waveRoutine = null;
            }

            _isWaitingForNextWave = false;
        }

        private void UntrackEnemy(Health enemyHealth)
        {
            if (enemyHealth == null)
            {
                return;
            }

            if (_enemyDeathHandlers.TryGetValue(enemyHealth, out Action deathHandler))
            {
                enemyHealth.OnDied -= deathHandler;
                _enemyDeathHandlers.Remove(enemyHealth);
            }
        }

        private void ClearEnemyDeathHandlers()
        {
            foreach (KeyValuePair<Health, Action> pair in _enemyDeathHandlers)
            {
                if (pair.Key != null)
                {
                    pair.Key.OnDied -= pair.Value;
                }
            }

            _enemyDeathHandlers.Clear();
        }

        private bool WasPauseKeyPressed()
        {
            if (pauseKey == Key.None || Keyboard.current == null)
            {
                return false;
            }

            return Keyboard.current[pauseKey].wasPressedThisFrame;
        }

        private void SetState(LevelState state)
        {
            if (State == state)
            {
                return;
            }

            State = state;
            OnStateChanged?.Invoke(State);
        }
    }
}
