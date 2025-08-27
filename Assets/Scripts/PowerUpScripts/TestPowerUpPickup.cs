using UnityEngine;

public class TestPowerUpPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // UI notification
            if (PowerUpUIManager.Instance != null)
            {
                PowerUpUIManager.Instance.ShowPowerUpNotification(gameObject.name);
            }
        }
    }
}