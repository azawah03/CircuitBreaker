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
    
    public int CurrentWave => currentWaveIndex + 1;
    public int TotalWaves => waves?.Length ?? 0;
    public bool IsWaveActive => isWaveActive;
    public bool AllWavesComplete => allWavesComplete;
    public int EnemiesRemaining => currentWaveEnemies.Count;

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
        
        Debug.Log($"Starting {waveConfig.waveName}");
        OnWaveStart?.Invoke(CurrentWave);

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
        
        isWaveActive = false;
        OnWaveComplete?.Invoke(CurrentWave);
        
        Debug.Log($"{waveConfig.waveName} completed!");
        
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

    bool AreAllEnemiesDefeated()
    {
        // Remove null references (destroyed enemies)
        currentWaveEnemies.RemoveAll(enemy => enemy == null);
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
}
