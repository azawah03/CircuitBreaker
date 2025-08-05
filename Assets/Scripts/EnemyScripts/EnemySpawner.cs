using System.Diagnostics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public EnemyType[] enemyTypes;
    public Transform[] spawnPoints;
    public Transform player;
    public float spawnInterval = 30f;
    public float spawnY = 0f; 

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnRandomEnemy();
            timer = 0f;
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
}
