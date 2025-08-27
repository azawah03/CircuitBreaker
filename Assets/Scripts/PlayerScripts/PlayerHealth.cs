using System.Diagnostics;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public delegate void OnHealthChanged(float current, float max);
    public event OnHealthChanged onHealthChanged;

    private bool isGameOver = false;
    private PowerUpManager powerUpManager;

    void Start()
    {
        powerUpManager = GetComponent<PowerUpManager>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        // Check if player has shield active
        if (powerUpManager != null && powerUpManager.hasShield)
        {
            // Shield blocks all damage
            return;
        }

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f && !isGameOver)
        {
            isGameOver = true;
            
            // Use GameManager if available, otherwise fall back to old system
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied();
            }
            else
            {
                Time.timeScale = 0f; // Fallback for old system
            }
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isGameOver = false;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void OnGUI()
    {
        if (isGameOver)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 60;
            style.normal.textColor = Color.red;
            style.alignment = TextAnchor.MiddleCenter;
            Rect rect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 30, 400, 100);
            GUI.Label(rect, "YOU LOST", style);
        }
    }
}