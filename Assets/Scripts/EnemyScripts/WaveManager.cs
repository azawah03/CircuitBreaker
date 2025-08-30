using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Configuration")]
    public WaveConfig[] waves;
    public bool autoStartWaves = true;
    public float timeBetweenWaves = 5f;
    
    [Header("Timed Waves")]
    public bool useTimedWaves = true;
    public float waveLength = 60f; // 1 minute per wave
    
    [Header("Spawning")]
    public EnemySpawner enemySpawner;
    
    [Header("Events")]
    public UnityEvent<int> OnWaveStart;
    public UnityEvent<int> OnWaveComplete;
    public UnityEvent OnAllWavesComplete;
    
    // Current state
    private int currentWaveIndex = 0;
    private bool isWaveActive = false;
    private bool allWavesComplete = false;
    private List<GameObject> currentWaveEnemies = new List<GameObject>();
    private Coroutine currentWaveCoroutine;
    private float currentWaveTimer = 0f;
    private float waveStartTime = 0f;
    
    public int CurrentWave => currentWaveIndex + 1;
    public int TotalWaves => waves?.Length ?? 0;
    public bool IsWaveActive => isWaveActive;
    public bool AllWavesComplete => allWavesComplete;
    public int EnemiesRemaining => currentWaveEnemies.Count;
    public float WaveTimeRemaining => useTimedWaves ? Mathf.Max(0, waveLength - (Time.time - waveStartTime)) : 0f;
    public float WaveProgress => useTimedWaves ? Mathf.Clamp01((Time.time - waveStartTime) / waveLength) : 0f;

    void Start()
    {
        if (enemySpawner == null)
            enemySpawner = FindObjectOfType<EnemySpawner>();
            
        if (autoStartWaves && waves.Length > 0)
        {
            StartCoroutine(StartFirstWaveDelayed());
        }
    }
    
    IEnumerator StartFirstWaveDelayed()
    {
        yield return new WaitForSeconds(2f); // Give time for game to initialize
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (allWavesComplete || currentWaveIndex >= waves.Length)
        {
            Debug.Log("All waves completed!");
            allWavesComplete = true;
            OnAllWavesComplete?.Invoke();
            return;
        }

        if (currentWaveCoroutine != null)
        {
            StopCoroutine(currentWaveCoroutine);
        }

        currentWaveCoroutine = StartCoroutine(ExecuteWave(waves[currentWaveIndex]));
    }

    IEnumerator ExecuteWave(WaveConfig waveConfig)
    {
        isWaveActive = true;
        currentWaveEnemies.Clear();
        waveStartTime = Time.time;
        
        Debug.Log($"Starting {waveConfig.waveName} (Wave {CurrentWave}) - Duration: {(useTimedWaves ? waveLength + " seconds" : "until enemies defeated")}");
        OnWaveStart?.Invoke(CurrentWave);

        if (useTimedWaves)
        {
            // Timed wave: spawn enemies continuously for the wave duration
            yield return StartCoroutine(ExecuteTimedWave(waveConfig));
        }
        else
        {
            // Original behavior: spawn all enemies then wait for completion
            yield return StartCoroutine(ExecuteEnemyCountWave(waveConfig));
        }

        isWaveActive = false;
        Debug.Log($"{waveConfig.waveName} completed!");
        OnWaveComplete?.Invoke(CurrentWave);
        
        // Move to next wave
        currentWaveIndex++;
        
        if (currentWaveIndex < waves.Length)
        {
            // Wait before starting next wave
            yield return new WaitForSeconds(waveConfig.timeBeforeNextWave);
            StartNextWave();
        }
        else
        {
            // All waves completed
            allWavesComplete = true;
            OnAllWavesComplete?.Invoke();
            Debug.Log("All waves completed! Victory!");
            
            // Trigger victory state if GameManager exists
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerVictory();
            }
        }
    }
    
    IEnumerator ExecuteTimedWave(WaveConfig waveConfig)
    {
        float waveEndTime = waveStartTime + waveLength;
        
        // Start spawning coroutine for continuous enemy spawning
        StartCoroutine(SpawnEnemiesContinuously(waveConfig, waveEndTime));
        
        // Wait for the wave duration
        while (Time.time < waveEndTime)
        {
            yield return null;
        }
        
        Debug.Log($"Wave {CurrentWave} time completed. Cleaning up remaining enemies...");
        
        // Optional: Clean up remaining enemies or let them continue to exist
        // You can choose to destroy them or let the player finish them off
        ClearRemainingEnemies();
    }
    
    IEnumerator ExecuteEnemyCountWave(WaveConfig waveConfig)
    {
        // Original enemy-count-based wave logic
        // Spawn all enemy types defined in this wave
        for (int i = 0; i < waveConfig.enemyWaves.Length; i++)
        {
            EnemyWaveData enemyWave = waveConfig.enemyWaves[i];
            
            // Wait for spawn delay if specified
            if (enemyWave.spawnDelay > 0)
            {
                yield return new WaitForSeconds(enemyWave.spawnDelay);
            }
            
            // Spawn enemies
            if (enemyWave.spawnSimultaneously)
            {
                // Spawn all enemies of this type at once
                for (int j = 0; j < enemyWave.enemyCount; j++)
                {
                    GameObject enemy = enemySpawner.SpawnSpecificEnemy(enemyWave.enemyType, waveConfig);
                    if (enemy != null)
                    {
                        currentWaveEnemies.Add(enemy);
                        Debug.Log($"Spawned enemy {j + 1}/{enemyWave.enemyCount}. Total enemies in wave: {currentWaveEnemies.Count}");
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to spawn enemy {j + 1} for wave {CurrentWave}");
                    }
                }
            }
            else
            {
                // Spawn enemies with interval
                for (int j = 0; j < enemyWave.enemyCount; j++)
                {
                    GameObject enemy = enemySpawner.SpawnSpecificEnemy(enemyWave.enemyType, waveConfig);
                    if (enemy != null)
                    {
                        currentWaveEnemies.Add(enemy);
                        Debug.Log($"Spawned enemy {j + 1}/{enemyWave.enemyCount} (Interval). Total enemies in wave: {currentWaveEnemies.Count}");
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to spawn enemy {j + 1} for wave {CurrentWave} (Interval)");
                    }
                    
                    if (j < enemyWave.enemyCount - 1) // Don't wait after the last enemy
                    {
                        yield return new WaitForSeconds(waveConfig.timeBetweenEnemySpawns);
                    }
                }
            }
        }

        // Wait for all enemies to be defeated
        yield return new WaitUntil(() => AreAllEnemiesDefeated());
    }
    
    IEnumerator SpawnEnemiesContinuously(WaveConfig waveConfig, float endTime)
    {
        Debug.Log($"Starting continuous spawning for Wave {CurrentWave} until {endTime - Time.time} seconds");
        
        while (Time.time < endTime)
        {
            // Spawn enemies from each enemy type in the wave configuration
            for (int i = 0; i < waveConfig.enemyWaves.Length; i++)
            {
                EnemyWaveData enemyWave = waveConfig.enemyWaves[i];
                
                GameObject enemy = enemySpawner.SpawnSpecificEnemy(enemyWave.enemyType, waveConfig);
                if (enemy != null)
                {
                    currentWaveEnemies.Add(enemy);
                    Debug.Log($"Continuous spawn: Total enemies in wave: {currentWaveEnemies.Count}");
                }
            }
            
            // Wait before spawning next batch
            float spawnInterval = waveConfig.timeBetweenEnemySpawns;
            // Make spawning faster in later waves
            spawnInterval = Mathf.Max(0.5f, spawnInterval - (currentWaveIndex * 0.2f));
            
            yield return new WaitForSeconds(spawnInterval);
        }
        
        Debug.Log($"Finished continuous spawning for Wave {CurrentWave}");
    }
    
    void ClearRemainingEnemies()
    {
        int enemiesCleared = 0;
        foreach (GameObject enemy in currentWaveEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
                enemiesCleared++;
            }
        }
        currentWaveEnemies.Clear();
        
        if (enemiesCleared > 0)
        {
            Debug.Log($"Cleared {enemiesCleared} remaining enemies at wave end");
        }
    }

    bool AreAllEnemiesDefeated()
    {
        // Remove null references (destroyed enemies)
        int beforeCount = currentWaveEnemies.Count;
        currentWaveEnemies.RemoveAll(enemy => enemy == null);
        
        if (beforeCount != currentWaveEnemies.Count)
        {
            Debug.Log($"Cleaned up {beforeCount - currentWaveEnemies.Count} destroyed enemies. Remaining: {currentWaveEnemies.Count}");
        }
        
        return currentWaveEnemies.Count == 0;
    }

    public void RegisterEnemyDestroyed(GameObject enemy)
    {
        currentWaveEnemies.Remove(enemy);
    }
    
    public void ForceNextWave()
    {
        if (currentWaveCoroutine != null)
        {
            StopCoroutine(currentWaveCoroutine);
        }
        
        // Clear current wave enemies
        foreach (GameObject enemy in currentWaveEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        currentWaveEnemies.Clear();
        
        isWaveActive = false;
        StartNextWave();
    }
    
    public void RestartWaves()
    {
        if (currentWaveCoroutine != null)
        {
            StopCoroutine(currentWaveCoroutine);
        }
        
        currentWaveIndex = 0;
        allWavesComplete = false;
        isWaveActive = false;
        currentWaveEnemies.Clear();
        
        if (waves.Length > 0)
        {
            StartCoroutine(StartFirstWaveDelayed());
        }
    }
    
    [ContextMenu("Auto-Configure Waves with Prefabs")]
    public void AutoConfigureWaves()
    {
        // Find enemy prefabs in the project
        GameObject[] enemyPrefabs = new GameObject[3];
        enemyPrefabs[0] = Resources.Load<GameObject>("kamikaze");
        enemyPrefabs[1] = Resources.Load<GameObject>("rangeBot");
        enemyPrefabs[2] = Resources.Load<GameObject>("Robot_Soldier");
        
        // If Resources.Load doesn't work, you'll need to manually assign these in Inspector
        // This is just a helper - the real fix is manual assignment
        
        if (waves == null || waves.Length == 0)
        {
            // Create 5 sample waves
            waves = new WaveConfig[5];
            for (int i = 0; i < 5; i++)
            {
                waves[i] = new WaveConfig();
                waves[i].waveName = $"Wave {i + 1}";
                waves[i].waveNumber = i + 1;
                waves[i].timeBetweenEnemySpawns = 1.5f;
                waves[i].timeBeforeNextWave = 5f;
                
                // Difficulty scaling
                waves[i].enemySpeedMultiplier = 1f + (i * 0.1f);
                waves[i].enemyHealthMultiplier = 1f + (i * 0.2f);
                waves[i].enemyDamageMultiplier = 1f + (i * 0.1f);
                
                // Create enemy wave data - start with one enemy type per wave
                waves[i].enemyWaves = new EnemyWaveData[1];
                waves[i].enemyWaves[0] = new EnemyWaveData();
                waves[i].enemyWaves[0].enemyCount = 3 + i; // 3, 4, 5, 6, 7 enemies per wave
                
                // Create EnemyType (you still need to assign prefab manually!)
                waves[i].enemyWaves[0].enemyType = new EnemyType();
                waves[i].enemyWaves[0].enemyType.moveSpeed = 3f + i;
                waves[i].enemyWaves[0].enemyType.damage = 10f + (i * 5f);
                waves[i].enemyWaves[0].enemyType.spawnY = 0f;
                // Note: prefab still needs to be assigned manually in inspector!
            }
        }
        
        Debug.Log("Auto-configured waves created! You still need to manually assign the prefabs in the Inspector.");
        Debug.Log("Drag kamikaze.prefab, rangeBot.prefab, or Robot_Soldier.prefab to the Enemy Type → Prefab fields");
    }
}
