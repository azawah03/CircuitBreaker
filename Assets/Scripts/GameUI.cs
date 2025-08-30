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
    private Coroutine currentStartNotificationCoroutine;
    private Coroutine currentCompleteNotificationCoroutine;

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
        
        // Debug: Press H to manually hide notifications (for testing)
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Manual hide notifications triggered");
            StopAllNotifications();
        }
        
        // Debug: Press T to test notification (for testing)
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Manual test notification triggered");
            OnWaveStart(99); // Test notification
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
        // Use GameManager's reference
        if (GameManager.Instance != null && GameManager.Instance.playerMovement != null)
        {
            playerMovement = GameManager.Instance.playerMovement;
        }

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
        // Stop any running notification coroutines
        if (currentStartNotificationCoroutine != null)
        {
            StopCoroutine(currentStartNotificationCoroutine);
            currentStartNotificationCoroutine = null;
        }
        
        if (currentCompleteNotificationCoroutine != null)
        {
            StopCoroutine(currentCompleteNotificationCoroutine);
            currentCompleteNotificationCoroutine = null;
        }
        
        // Hide notifications
        if (waveStartNotification != null)
            waveStartNotification.SetActive(false);
        if (waveCompleteNotification != null)
            waveCompleteNotification.SetActive(false);
        
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
            if (waveManager.IsWaveActive && waveManager.WaveTimeRemaining > 0)
            {
                int minutes = Mathf.FloorToInt(waveManager.WaveTimeRemaining / 60);
                int seconds = Mathf.FloorToInt(waveManager.WaveTimeRemaining % 60);
                waveText.text = $"Wave {waveManager.CurrentWave}/{waveManager.TotalWaves} - {minutes:00}:{seconds:00}";
            }
            else
            {
                waveText.text = $"Wave {waveManager.CurrentWave}/{waveManager.TotalWaves}";
            }
        }
        
        // Update enemies remaining (keep this for reference)
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
        
        // Update wave progress bar - now shows time progress within current wave
        if (waveProgressSlider != null)
        {
            float progress = 0f;
            if (waveManager.IsWaveActive)
            {
                // Show progress within the current wave (0 to 1 over 60 seconds)
                progress = waveManager.WaveProgress;
            }
            else
            {
                // Show overall progress through all waves
                progress = (float)(waveManager.CurrentWave - 1) / (float)waveManager.TotalWaves;
            }
            waveProgressSlider.value = progress;
        }
        
        // Update next wave countdown text (if you want to show time until next wave)
        if (nextWaveCountdownText != null)
        {
            if (!waveManager.IsWaveActive && !waveManager.AllWavesComplete)
            {
                nextWaveCountdownText.text = "Next wave starting...";
            }
            else if (waveManager.IsWaveActive)
            {
                float timeRemaining = waveManager.WaveTimeRemaining;
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                nextWaveCountdownText.text = $"Wave ends in: {minutes:00}:{seconds:00}";
            }
            else
            {
                nextWaveCountdownText.text = "";
            }
        }
    }
    
    void OnWaveStart(int waveNumber)
    {
        Debug.Log($"OnWaveStart called for wave {waveNumber}");
        
        if (waveStartNotification != null)
        {
            // Immediately stop and hide any existing notifications
            StopAllNotifications();
            
            // Show the notification
            ShowWaveNotification(waveStartNotification, $"Wave {waveNumber} Starting!");
        }
        else
        {
            Debug.LogError("waveStartNotification is null!");
        }
    }
    
    void OnWaveComplete(int waveNumber)
    {
        Debug.Log($"OnWaveComplete called for wave {waveNumber}");
        
        if (waveCompleteNotification != null)
        {
            // Immediately stop and hide any existing notifications  
            StopAllNotifications();
            
            // Show the notification
            ShowWaveNotification(waveCompleteNotification, $"Wave {waveNumber} Complete!");
        }
        else
        {
            Debug.LogError("waveCompleteNotification is null!");
        }
    }
    
    void OnAllWavesComplete()
    {
        Debug.Log("OnAllWavesComplete called");
        
        if (waveCompleteNotification != null)
        {
            // Immediately stop and hide any existing notifications
            StopAllNotifications();
            
            // Show the notification  
            ShowWaveNotification(waveCompleteNotification, "All Waves Complete!\nVictory!");
        }
    }
    
    void StopAllNotifications()
    {
        // Stop any running coroutines
        if (currentStartNotificationCoroutine != null)
        {
            StopCoroutine(currentStartNotificationCoroutine);
            currentStartNotificationCoroutine = null;
        }
        
        if (currentCompleteNotificationCoroutine != null)
        {
            StopCoroutine(currentCompleteNotificationCoroutine);
            currentCompleteNotificationCoroutine = null;
        }
        
        // Immediately hide all notifications
        if (waveStartNotification != null)
        {
            waveStartNotification.SetActive(false);
            Debug.Log("Forced hide waveStartNotification");
        }
        
        if (waveCompleteNotification != null)
        {
            waveCompleteNotification.SetActive(false);
            Debug.Log("Forced hide waveCompleteNotification");
        }
    }
    
    void ShowWaveNotification(GameObject notification, string message)
    {
        // Set the message
        TextMeshProUGUI notificationText = notification.GetComponentInChildren<TextMeshProUGUI>();
        if (notificationText != null)
        {
            notificationText.text = message;
            Debug.Log($"Set notification text to: {message}");
        }
        else
        {
            Debug.LogError($"No TextMeshProUGUI found in {notification.name}");
        }
        
        // Show notification
        notification.SetActive(true);
        Debug.Log($"Activated notification: {notification.name}");
        
        // Start timer to hide it
        if (notification == waveStartNotification)
        {
            currentStartNotificationCoroutine = StartCoroutine(HideNotificationAfterDelay(notification, 2f, true));
        }
        else
        {
            currentCompleteNotificationCoroutine = StartCoroutine(HideNotificationAfterDelay(notification, 2f, false));
        }
    }
    
    System.Collections.IEnumerator HideNotificationAfterDelay(GameObject notification, float delay, bool isStartNotification)
    {
        Debug.Log($"Starting timer to hide {notification.name} after {delay} seconds");
        
        yield return new WaitForSeconds(delay);
        
        if (notification != null)
        {
            notification.SetActive(false);
            Debug.Log($"Timer expired - hiding {notification.name}");
        }
        
        // Clear the coroutine reference
        if (isStartNotification)
        {
            currentStartNotificationCoroutine = null;
        }
        else
        {
            currentCompleteNotificationCoroutine = null;
        }
    }
    
    #endregion
}
