using UnityEngine;

[System.Serializable]
public class WaveConfig
{
    [Header("Wave Information")]
    public string waveName = "Wave 1";
    public int waveNumber = 1;
    
    [Header("Enemy Configuration")]
    public EnemyWaveData[] enemyWaves;
    
    [Header("Timing")]
    public float timeBetweenEnemySpawns = 2f;
    public float timeBeforeNextWave = 10f;
    
    [Header("Difficulty Scaling")]
    public float enemySpeedMultiplier = 1f;
    public float enemyHealthMultiplier = 1f;
    public float enemyDamageMultiplier = 1f;
}

[System.Serializable]
public class EnemyWaveData
{
    public EnemyType enemyType;
    public int enemyCount = 5;
    public float spawnDelay = 0f; // Delay before starting to spawn this enemy type
    public bool spawnSimultaneously = false; // If true, spawn all at once
}
