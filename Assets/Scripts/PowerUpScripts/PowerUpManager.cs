using UnityEngine;
using System.Collections;

public class PowerUpManager : MonoBehaviour
{
    [Header("Power-Up Durations")]
    public float rapidFireDuration = 10f;
    public float speedBoostDuration = 10f;
    public float shieldDuration = 5f;

    [Header("Power-Up Effects")]
    public float rapidFireRate = 0.25f; // Time between shots
    public float speedBoostMultiplier = 1.5f;

    [Header("Visual Effects")]
    public GameObject shieldVisual; 

    // References
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;
    private PlayerHealth playerHealth;

    // Original values
    private float originalFireRate;
    private float originalMoveSpeed;
    private float originalSprintSpeed;

    // Power-up states
    private bool hasRapidFire = false;
    private bool hasSpeedBoost = false;
    public bool hasShield = false; 

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        playerHealth = GetComponent<PlayerHealth>();

        // Store original values
        originalMoveSpeed = playerMovement.moveSpeed;
        originalSprintSpeed = playerMovement.sprintSpeed;
        originalFireRate = playerShooting.fireRate;

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            string powerUpName = other.gameObject.name;

            // Show UI notification first
            if (PowerUpUIManager.Instance != null)
            {
                PowerUpUIManager.Instance.ShowPowerUpNotification(powerUpName);
            }

            // Then activate power-up
            if (powerUpName.Contains("RapidFire"))
            {
                StartCoroutine(ActivateRapidFire());
            }
            else if (powerUpName.Contains("SpeedBoost"))
            {
                StartCoroutine(ActivateSpeedBoost());
            }
            else if (powerUpName.Contains("Shield"))
            {
                StartCoroutine(ActivateShield());
            }

            // Finally destroy
            Destroy(other.gameObject);
        }
    }

    IEnumerator ActivateRapidFire()
    {
        if (hasRapidFire) yield break;

        hasRapidFire = true;
        playerShooting.fireRate = rapidFireRate;

        yield return new WaitForSeconds(rapidFireDuration);

        playerShooting.fireRate = originalFireRate;
        hasRapidFire = false;
    }

    IEnumerator ActivateSpeedBoost()
    {
        if (hasSpeedBoost) yield break;

        hasSpeedBoost = true;
        playerMovement.moveSpeed = originalMoveSpeed * speedBoostMultiplier;
        playerMovement.sprintSpeed = originalSprintSpeed * speedBoostMultiplier;

        yield return new WaitForSeconds(speedBoostDuration);

        playerMovement.moveSpeed = originalMoveSpeed;
        playerMovement.sprintSpeed = originalSprintSpeed;
        hasSpeedBoost = false;
    }

    IEnumerator ActivateShield()
    {
        if (hasShield) yield break;

        hasShield = true;
        shieldVisual.SetActive(true);

        yield return new WaitForSeconds(shieldDuration);

        hasShield = false;
        shieldVisual.SetActive(false);
    }
}