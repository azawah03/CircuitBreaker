using UnityEngine;

public class TestPowerUpPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Show UI notification
            if (PowerUpUIManager.Instance != null)
            {
                PowerUpUIManager.Instance.ShowPowerUpNotification(gameObject.name);
            }

            // Destroy the power-up
            Destroy(gameObject);
        }
    }
}