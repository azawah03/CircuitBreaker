using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;
    public Slider healthSlider;
    public Slider staminaSlider;  
    private PlayerMovement playerMovement;  
    public GameObject pauseMenu;
    public GameObject gameOverScreen;
    public GameObject victoryScreen;
    
    [Header("Wave System UI")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemiesRemainingText;
    public TextMeshProUGUI nextWaveCountdownText;
    public Slider waveProgressSlider;
    public GameObject waveStartNotification;
    public GameObject waveCompleteNotification;
    
    private WaveManager waveManager;

    void Start()
    {
        // Subscribe to GameManager events if available
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnLivesChanged += UpdateLives;
        }

        // Find and subscribe to WaveManager events
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnWaveStart.AddListener(OnWaveStart);
            waveManager.OnWaveComplete.AddListener(OnWaveComplete);
            waveManager.OnAllWavesComplete.AddListener(OnAllWavesComplete);
        }

        // Find player movement for stamina tracking
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }
        
        // Initialize wave UI
        InitializeWaveUI();
    }

    void Update()
    {
        // Update UI elements if GameManager is available
        if (GameManager.Instance != null)
        {
            UpdateTimer();
            UpdateHealthBar();
            UpdateStaminaBar();
        }
        
        // Update wave UI
        UpdateWaveUI();
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
                    victoryScreen.SetActive(true);
                    UnlockCursor();
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

    void UpdateStaminaBar()
    {
        if (staminaSlider != null && playerMovement != null)
        {
            staminaSlider.maxValue = playerMovement.maxStamina;
            staminaSlider.value = playerMovement.stamina;
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

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnLivesChanged -= UpdateLives;
        }
        
        // Unsubscribe from wave manager events
        if (waveManager != null)
        {
            waveManager.OnWaveStart.RemoveListener(OnWaveStart);
            waveManager.OnWaveComplete.RemoveListener(OnWaveComplete);
            waveManager.OnAllWavesComplete.RemoveListener(OnAllWavesComplete);
        }
    }
    
    #region Wave UI Methods
    
    void InitializeWaveUI()
    {
        // Hide notification panels initially
        if (waveStartNotification != null)
            waveStartNotification.SetActive(false);
        if (waveCompleteNotification != null)
            waveCompleteNotification.SetActive(false);
    }
    
    void UpdateWaveUI()
    {
        if (waveManager == null) return;
        
        // Update wave information
        if (waveText != null)
        {
            waveText.text = $"Wave {waveManager.CurrentWave}/{waveManager.TotalWaves}";
        }
        
        // Update enemies remaining
        if (enemiesRemainingText != null)
        {
            if (waveManager.IsWaveActive)
            {
                enemiesRemainingText.text = $"Enemies: {waveManager.EnemiesRemaining}";
            }
            else
            {
                enemiesRemainingText.text = "";
            }
        }
        
        // Update wave progress bar
        if (waveProgressSlider != null)
        {
            float progress = (float)(waveManager.CurrentWave - 1) / waveManager.TotalWaves;
            waveProgressSlider.value = progress;
        }
    }
    
    void OnWaveStart(int waveNumber)
    {
        if (waveStartNotification != null)
        {
            StartCoroutine(ShowNotification(waveStartNotification, $"Wave {waveNumber} Starting!", 2f));
        }
    }
    
    void OnWaveComplete(int waveNumber)
    {
        if (waveCompleteNotification != null)
        {
            StartCoroutine(ShowNotification(waveCompleteNotification, $"Wave {waveNumber} Complete!", 2f));
        }
    }
    
    void OnAllWavesComplete()
    {
        if (waveCompleteNotification != null)
        {
            StartCoroutine(ShowNotification(waveCompleteNotification, "All Waves Complete!\nVictory!", 3f));
        }
    }
    
    System.Collections.IEnumerator ShowNotification(GameObject notification, string message, float duration)
    {
        // Find text component and set message
        TextMeshProUGUI notificationText = notification.GetComponentInChildren<TextMeshProUGUI>();
        if (notificationText != null)
        {
            notificationText.text = message;
        }
        
        // Show notification
        notification.SetActive(true);
        
        // Wait for duration
        yield return new WaitForSeconds(duration);
        
        // Hide notification
        notification.SetActive(false);
    }
    
    #endregion
}
