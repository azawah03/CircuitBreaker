using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f; // Time between shots

    private float nextFireTime = 0f;

    void Update()
    {
        // Only allow shooting when game is in Playing state
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Playing)
            return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + fireRate;
        }
    }
}