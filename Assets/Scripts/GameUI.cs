using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("UI References")]
    public Text scoreText;
    public Text livesText;
    public TextMeshProUGUI timerText;
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

            // count down
            float timeRemaining = 300f - gameTimer;

            // Check if time is up
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                // You survived 5 minutes
                if (GameManager.Instance.currentState == GameState.Playing)
                {
                    GameManager.Instance.Victory();
                }
            }

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
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
