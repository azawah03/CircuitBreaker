using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Power-Up Prefabs")]
    public GameObject rapidFirePrefab;
    public GameObject speedBoostPrefab;
    public GameObject shieldPrefab;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float spawnInterval = 30f;
    public bool spawnOnStart = false;

    private List<GameObject> activePickups = new List<GameObject>();

    void Start()
    {
        if (spawnOnStart)
            SpawnRandomPowerUp();

        StartCoroutine(SpawnPowerUps());
    }

    IEnumerator SpawnPowerUps()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Only spawn if game is playing
            if (GameManager.Instance != null && GameManager.Instance.currentState == GameState.Playing)
            {
                SpawnRandomPowerUp();
            }
        }
    }

    void SpawnRandomPowerUp()
    {
        // Clean up any destroyed pickups
        activePickups.RemoveAll(item => item == null);

        // Find available spawn points (not occupied)
        List<Transform> availablePoints = new List<Transform>();
        foreach (Transform point in spawnPoints)
        {
            bool occupied = false;
            foreach (GameObject pickup in activePickups)
            {
                if (pickup != null && Vector3.Distance(pickup.transform.position, point.position) < 1f)
                {
                    occupied = true;
                    break;
                }
            }
            if (!occupied)
                availablePoints.Add(point);
        }

        if (availablePoints.Count == 0)
            return; // All spawn points occupied

        // Choose random spawn point
        Transform spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];

        // Choose random power-up type
        GameObject[] powerUpTypes = { rapidFirePrefab, speedBoostPrefab, shieldPrefab };
        GameObject randomPowerUp = powerUpTypes[Random.Range(0, powerUpTypes.Length)];

        // Spawn the power-up
        GameObject newPickup = Instantiate(randomPowerUp, spawnPoint.position, Quaternion.identity);
        activePickups.Add(newPickup);
    }
}