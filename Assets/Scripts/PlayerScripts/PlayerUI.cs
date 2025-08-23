using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Slider healthBar;
    public Slider staminaBar;
    public TextMeshProUGUI ammoText;
    private PlayerMovement movement;
    private PlayerHealth health;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (movement != null && staminaBar != null)
        {
            staminaBar.maxValue = movement.maxStamina;
            staminaBar.value = movement.stamina;
        }

        if (health != null)
        {
            healthBar.maxValue = health.maxHealth;
            healthBar.value = health.currentHealth;
        }

        ammoText.text = "Ammo: ∞";
    }
}