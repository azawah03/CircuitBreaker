using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Victory,
    Loading
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State Management")]
    public GameState currentState = GameState.MainMenu;
    public bool debugMode = false;

    [Header("Player Management")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;

    [Header("Scene Management")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "GameScene";
    public GameObject loadingScreen;
    public UnityEngine.UI.Slider loadingProgressBar;

    [Header("Player Data")]
    public int playerScore = 0;
    public int playerLives = 3;
    public int maxLives = 3;
    public float gameTimer = 0f;
    public bool isTimerActive = false;

    [Header("UI Management")]
    public GameObject mainMenuUI;
    public GameObject gameUI;
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject victoryUI;
    public UnityEngine.UI.Text scoreText;
    public UnityEngine.UI.Text livesText;
    public TMPro.TextMeshProUGUI timerText;
    public UnityEngine.UI.Slider healthSlider;

    [Header("Audio Management")]
    public AudioSource backgroundMusicSource;
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;
    public AudioClip victoryMusic;
    public AudioClip gameOverMusic;
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;

    [Header("Save/Load System")]
    public bool autoSave = true;
    public float autoSaveInterval = 30f;
    private float autoSaveTimer = 0f;

    // Game Data Structure
    [System.Serializable]
    public class GameData
    {
        public int highScore;
        public int totalPlayTime;
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        public string lastCheckpoint;
        public Vector3 lastPlayerPosition;
    }

    public GameData gameData;

    // Events
    public System.Action<GameState> OnGameStateChanged;
    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnLivesChanged;

    void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadGameData();
        ChangeGameState(GameState.MainMenu);
    }

    void Update()
    {
        HandleGameStateUpdates();
        HandleInput();
        HandleAutoSave();
        UpdateUI();
    }

    #region Game State Management
    void InitializeGameManager()
    {
        gameData = new GameData();
        
        // Find player components if not assigned
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerShooting == null)
            playerShooting = FindObjectOfType<PlayerShooting>();

        // Initialize audio
        if (backgroundMusicSource == null)
        {
            GameObject audioObj = new GameObject("BackgroundMusic");
            audioObj.transform.SetParent(transform);
            backgroundMusicSource = audioObj.AddComponent<AudioSource>();
            backgroundMusicSource.loop = true;
            backgroundMusicSource.playOnAwake = false;
        }

        Debug.Log("GameManager initialized successfully");
    }

    public void ChangeGameState(GameState newState)
    {
        GameState previousState = currentState;
        currentState = newState;

        if (debugMode)
            Debug.Log($"Game State Changed: {previousState} -> {newState}");

        HandleStateTransition(previousState, newState);
        OnGameStateChanged?.Invoke(newState);
    }

    void HandleStateTransition(GameState from, GameState to)
    {
        // Exit previous state
        switch (from)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                isTimerActive = false;
                break;
            case GameState.Paused:
                Time.timeScale = 1f;
                break;
        }

        // Enter new state
        switch (to)
        {
            case GameState.MainMenu:
                ShowUI(mainMenuUI);
                PlayBackgroundMusic(mainMenuMusic);
                Time.timeScale = 1f;
                break;

            case GameState.Playing:
                ShowUI(gameUI);
                PlayBackgroundMusic(gameplayMusic);
                Time.timeScale = 1f;
                isTimerActive = true;
                EnablePlayerControls(true);
                break;

            case GameState.Paused:
                ShowUI(pauseMenuUI);
                Time.timeScale = 0f;
                EnablePlayerControls(false);
                break;

            case GameState.GameOver:
                ShowUI(gameOverUI);
                PlayBackgroundMusic(gameOverMusic);
                Time.timeScale = 0f;
                isTimerActive = false;
                EnablePlayerControls(false);
                SaveGameData();
                break;

            case GameState.Victory:
                ShowUI(victoryUI);
                PlayBackgroundMusic(victoryMusic);
                Time.timeScale = 0f;
                isTimerActive = false;
                EnablePlayerControls(false);
                SaveGameData();
                break;

            case GameState.Loading:
                ShowUI(loadingScreen);
                break;
        }
    }

    void HandleGameStateUpdates()
    {
        switch (currentState)
        {
            case GameState.Playing:
                if (isTimerActive)
                    gameTimer += Time.deltaTime;
                
                // Check for game over conditions
                if (playerHealth != null && playerHealth.currentHealth <= 0)
                {
                    PlayerDied();
                }
                break;
        }
    }

    void HandleInput()
    {
        // Pause/Resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
                PauseGame();
            else if (currentState == GameState.Paused)
                ResumeGame();
        }

        // Debug commands (only in debug mode)
        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.F1))
                AddScore(100);
            if (Input.GetKeyDown(KeyCode.F2))
                AddLives(1);
            if (Input.GetKeyDown(KeyCode.F3))
                GameOver();
        }
    }
    #endregion

    #region Scene Management
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        ChangeGameState(GameState.Loading);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            if (loadingProgressBar != null)
                loadingProgressBar.value = progress;

            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f); // Minimum loading time
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Scene loaded, determine new state
        if (sceneName == mainMenuSceneName)
            ChangeGameState(GameState.MainMenu);
        else
            ChangeGameState(GameState.Playing);
    }

    public void RestartLevel()
    {
        ResetPlayerData();
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        LoadScene(mainMenuSceneName);
    }
    #endregion

    #region Player Data Management
    public void AddScore(int points)
    {
        playerScore += points;
        OnScoreChanged?.Invoke(playerScore);

        if (playerScore > gameData.highScore)
        {
            gameData.highScore = playerScore;
        }

        if (debugMode)
            Debug.Log($"Score added: {points}. Total: {playerScore}");
    }

    public void AddLives(int lives)
    {
        playerLives = Mathf.Clamp(playerLives + lives, 0, maxLives);
        OnLivesChanged?.Invoke(playerLives);

        if (debugMode)
            Debug.Log($"Lives changed: {lives}. Total: {playerLives}");
    }

    public void PlayerDied()
    {
        AddLives(-1);

        if (playerLives > 0)
        {
            // Respawn player
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            // Game Over
            GameOver();
        }
    }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2f);

        if (playerSpawnPoint != null && playerHealth != null)
        {
            // Reset player position
            playerMovement.transform.position = playerSpawnPoint.position;
            
            // Reset player health
            playerHealth.ResetHealth();
        }
    }

    void ResetPlayerData()
    {
        playerScore = 0;
        playerLives = maxLives;
        gameTimer = 0f;
        OnScoreChanged?.Invoke(playerScore);
        OnLivesChanged?.Invoke(playerLives);
    }
    #endregion

    #region UI Management
    void ShowUI(GameObject uiToShow)
    {
        // Hide all UI panels
        if (mainMenuUI) mainMenuUI.SetActive(false);
        if (gameUI) gameUI.SetActive(false);
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);
        if (victoryUI) victoryUI.SetActive(false);
        if (loadingScreen) loadingScreen.SetActive(false);

        // Show requested UI
        if (uiToShow) uiToShow.SetActive(true);
    }

    void UpdateUI()
    {
        if (currentState == GameState.Playing)
        {
            // Update score
            if (scoreText)
                scoreText.text = "Score: " + playerScore.ToString();

            // Update lives
            if (livesText)
                livesText.text = "Lives: " + playerLives.ToString();

            // Update timer
            if (timerText)
            {
                int minutes = Mathf.FloorToInt(gameTimer / 60);
                int seconds = Mathf.FloorToInt(gameTimer % 60);
                timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
            }

            // Update health slider
            if (healthSlider && playerHealth)
            {
                healthSlider.value = (float)playerHealth.currentHealth / playerHealth.maxHealth;
            }
        }
    }

    void EnablePlayerControls(bool enable)
    {
        if (playerMovement) playerMovement.enabled = enable;
        if (playerShooting) playerShooting.enabled = enable;
    }
    #endregion

    #region Audio Management
    void PlayBackgroundMusic(AudioClip clip)
    {
        if (backgroundMusicSource && clip)
        {
            backgroundMusicSource.clip = clip;
            backgroundMusicSource.volume = musicVolume * masterVolume;
            backgroundMusicSource.Play();
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = masterVolume;
        UpdateAudioVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateAudioVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateAudioVolumes();
    }

    void UpdateAudioVolumes()
    {
        if (backgroundMusicSource)
            backgroundMusicSource.volume = musicVolume * masterVolume;

        // Update all AudioSources with SFX volume
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            if (source != backgroundMusicSource)
            {
                source.volume = sfxVolume * masterVolume;
            }
        }
    }
    #endregion

    #region Save/Load System
    void HandleAutoSave()
    {
        if (autoSave && currentState == GameState.Playing)
        {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval)
            {
                SaveGameData();
                autoSaveTimer = 0f;
            }
        }
    }

    public void SaveGameData()
    {
        gameData.highScore = Mathf.Max(gameData.highScore, playerScore);
        gameData.totalPlayTime += Mathf.FloorToInt(gameTimer);
        gameData.masterVolume = masterVolume;
        gameData.musicVolume = musicVolume;
        gameData.sfxVolume = sfxVolume;

        if (playerMovement)
            gameData.lastPlayerPosition = playerMovement.transform.position;

        string jsonData = JsonUtility.ToJson(gameData, true);
        PlayerPrefs.SetString("GameData", jsonData);
        PlayerPrefs.Save();

        if (debugMode)
            Debug.Log("Game data saved");
    }

    public void LoadGameData()
    {
        if (PlayerPrefs.HasKey("GameData"))
        {
            string jsonData = PlayerPrefs.GetString("GameData");
            gameData = JsonUtility.FromJson<GameData>(jsonData);

            // Apply loaded settings
            SetMasterVolume(gameData.masterVolume);
            SetMusicVolume(gameData.musicVolume);
            SetSFXVolume(gameData.sfxVolume);

            if (debugMode)
                Debug.Log("Game data loaded");
        }
        else
        {
            // Create default data
            gameData = new GameData
            {
                highScore = 0,
                totalPlayTime = 0,
                masterVolume = 1f,
                musicVolume = 0.7f,
                sfxVolume = 0.8f
            };

            if (debugMode)
                Debug.Log("Created new game data");
        }
    }

    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey("GameData");
        gameData = new GameData();
        Debug.Log("Save data deleted");
    }
    #endregion

    #region Public Methods (UI Buttons)
    public void StartGame()
    {
        ResetPlayerData();
        ChangeGameState(GameState.Playing);
    }

    public void PauseGame()
    {
        if (currentState == GameState.Playing)
            ChangeGameState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
            ChangeGameState(GameState.Playing);
    }

    public void GameOver()
    {
        ChangeGameState(GameState.GameOver);
    }

    public void Victory()
    {
        ChangeGameState(GameState.Victory);
    }

    public void QuitGame()
    {
        SaveGameData();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    #endregion

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && currentState == GameState.Playing)
            PauseGame();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && currentState == GameState.Playing)
            PauseGame();
    }

    void OnDestroy()
    {
        SaveGameData();
    }
    
    public void TriggerVictory()
    {
        ChangeGameState(GameState.Victory);
    }
}
