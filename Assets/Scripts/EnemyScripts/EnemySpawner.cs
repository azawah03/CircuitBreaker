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
            Debug.LogWarning($"Cannot spawn enemy: enemyType={enemyType}, prefab={enemyType?.prefab}, spawnPoints.Length={spawnPoints?.Length}");
            return null;
        }

        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        // Override Y position
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.y = enemyType.spawnY;

        GameObject obj = Instantiate(enemyType.prefab, spawnPos, Quaternion.identity);
        Debug.Log($"Successfully spawned {enemyType.prefab.name} at {spawnPos}");

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
    
    [ContextMenu("Create 5 Spawn Points Around Player")]
    public void CreateSpawnPointsAroundPlayer()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is missing! Assign player first.");
            return;
        }
        
        // Create a parent GameObject for spawn points if it doesn't exist
        GameObject spawnParent = GameObject.Find("SpawnPoints");
        if (spawnParent == null)
        {
            spawnParent = new GameObject("SpawnPoints");
        }
        
        // Clear existing spawn points array
        spawnPoints = new Transform[5];
        
        // Distance from player and Y level
        float spawnDistance = 15f; // Adjust this based on your arena size
        float spawnY = player.position.y; // Same Y level as player
        
        // Create 5 spawn points in a circle around the player
        for (int i = 0; i < 5; i++)
        {
            // Calculate position in circle
            float angle = i * 72f; // 360/5 = 72 degrees between points
            float radian = angle * Mathf.Deg2Rad;
            
            Vector3 spawnPos = new Vector3(
                player.position.x + Mathf.Cos(radian) * spawnDistance,
                spawnY,
                player.position.z + Mathf.Sin(radian) * spawnDistance
            );
            
            // Create spawn point GameObject
            GameObject spawnPoint = new GameObject($"SpawnPoint{i + 1}");
            spawnPoint.transform.position = spawnPos;
            spawnPoint.transform.parent = spawnParent.transform;
            
            // Add to array
            spawnPoints[i] = spawnPoint.transform;
            
            Debug.Log($"Created {spawnPoint.name} at position {spawnPos}");
        }
        
        Debug.Log("Created 5 spawn points around player!");
    }
}
