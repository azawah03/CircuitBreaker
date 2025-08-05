using UnityEngine;

[System.Serializable]
public class EnemyType
{
    public GameObject prefab;
    public float moveSpeed = 3f;
    public float damage = 10f;
    public GameObject explosionEffect; // for kamikaze only
    public float spawnY = 0f;
}
