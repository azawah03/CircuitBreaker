using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public EnemyType[] enemyTypes;
    public Transform[] spawnPoints;
    public Transform player;
    public float spawnInterval = 30f;
    public float spawnY = 0f; 
    
    [Header("Wave System")]
    public bool useWaveSystem = true;
    public WaveManager waveManager;

    private float timer;

    void Update()
    {
        // Only use timer-based spawning if not using wave system
        if (!useWaveSystem || waveManager == null)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnRandomEnemy();
                timer = 0f;
            }
        }
    }

    void SpawnRandomEnemy()
    {
        if (enemyTypes == null || enemyTypes.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        EnemyType type = enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Length)];
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        // Override Y position
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.y = type.spawnY; 


        GameObject obj = Instantiate(type.prefab, spawnPos, Quaternion.identity);

        // Assign target and parameters
        EnemyAI ai = obj.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.target = player;
            ai.moveSpeed = type.moveSpeed;
        }

        KamikazeBot kamikaze = obj.GetComponent<KamikazeBot>();
        if (kamikaze != null)
        {
            kamikaze.Setup(player, type.moveSpeed, type.damage, type.explosionEffect);
        }

        RangedBot ranged = obj.GetComponent<RangedBot>();
        if (ranged != null)
        {
            ranged.target = player;
        }
    }
    
    // Enhanced method for wave-based spawning
    public GameObject SpawnSpecificEnemy(EnemyType enemyType, WaveConfig waveConfig = null)
    {
        if (enemyType?.prefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Cannot spawn enemy: missing prefab or spawn points");
            return null;
        }

        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        // Override Y position
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.y = enemyType.spawnY;

        GameObject obj = Instantiate(enemyType.prefab, spawnPos, Quaternion.identity);

        // Apply wave-based difficulty scaling
        float speedMultiplier = waveConfig?.enemySpeedMultiplier ?? 1f;
        float healthMultiplier = waveConfig?.enemyHealthMultiplier ?? 1f;
        float damageMultiplier = waveConfig?.enemyDamageMultiplier ?? 1f;

        // Assign target and parameters
        EnemyAI ai = obj.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.target = player;
            ai.moveSpeed = enemyType.moveSpeed * speedMultiplier;
            
            // Apply health scaling if the enemy has a health component
            PlayerHealth enemyHealth = obj.GetComponent<PlayerHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.maxHealth = Mathf.RoundToInt(enemyHealth.maxHealth * healthMultiplier);
                enemyHealth.currentHealth = enemyHealth.maxHealth;
            }
        }

        KamikazeBot kamikaze = obj.GetComponent<KamikazeBot>();
        if (kamikaze != null)
        {
            float scaledDamage = enemyType.damage * damageMultiplier;
            float scaledSpeed = enemyType.moveSpeed * speedMultiplier;
            kamikaze.Setup(player, scaledSpeed, scaledDamage, enemyType.explosionEffect);
        }

        RangedBot ranged = obj.GetComponent<RangedBot>();
        if (ranged != null)
        {
            ranged.target = player;
            // You can add damage scaling here if RangedBot has damage properties
        }
        
        // Register enemy with wave manager for tracking
        if (waveManager != null)
        {
            RegisterEnemyWithWaveManager(obj);
        }

        return obj;
    }
    
    void RegisterEnemyWithWaveManager(GameObject enemy)
    {
        // Add a component to track when this enemy is destroyed
        WaveEnemyTracker tracker = enemy.AddComponent<WaveEnemyTracker>();
        tracker.Initialize(waveManager);
    }
}
