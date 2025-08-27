using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("UI References")]
    public Text scoreText;
    public Text livesText;
    public Text timerText;
    public Slider healthSlider;
    public GameObject pauseMenu;
    public GameObject gameOverScreen;

    void Start()
    {
        // Subscribe to GameManager events if available
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnLivesChanged += UpdateLives;
        }
    }

    void Update()
    {
        // Update UI elements if GameManager is available
        if (GameManager.Instance != null)
        {
            UpdateTimer();
            UpdateHealthBar();
        }
    }

    void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void UpdateLives(int lives)
    {
        if (livesText != null)
            livesText.text = "Lives: " + lives;
    }

    void UpdateTimer()
    {
        if (timerText != null && GameManager.Instance != null)
        {
            float gameTimer = GameManager.Instance.gameTimer;
            int minutes = Mathf.FloorToInt(gameTimer / 60);
            int seconds = Mathf.FloorToInt(gameTimer % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null && GameManager.Instance != null && GameManager.Instance.playerHealth != null)
        {
            PlayerHealth playerHealth = GameManager.Instance.playerHealth;
            healthSlider.value = playerHealth.currentHealth / playerHealth.maxHealth;
        }
    }

    // Button Methods
    public void ResumeGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
    }

    public void RestartGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartLevel();
    }

    public void MainMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoadMainMenu();
    }

    public void QuitGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.QuitGame();
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnLivesChanged -= UpdateLives;
        }
    }
}
