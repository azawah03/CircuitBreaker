using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemiesRemainingText;
    public TextMeshProUGUI nextWaveCountdownText;
    public Slider waveProgressBar;
    public GameObject waveStartPanel;
    public GameObject waveCompletePanel;
    
    [Header("Animation")]
    public float panelDisplayDuration = 3f;
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private WaveManager waveManager;
    private CanvasGroup waveStartCanvasGroup;
    private CanvasGroup waveCompleteCanvasGroup;
    
    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        
        if (waveManager != null)
        {
            waveManager.OnWaveStart.AddListener(OnWaveStart);
            waveManager.OnWaveComplete.AddListener(OnWaveComplete);
            waveManager.OnAllWavesComplete.AddListener(OnAllWavesComplete);
        }
        
        // Setup canvas groups for animations
        if (waveStartPanel != null)
        {
            waveStartCanvasGroup = waveStartPanel.GetComponent<CanvasGroup>();
            if (waveStartCanvasGroup == null)
                waveStartCanvasGroup = waveStartPanel.AddComponent<CanvasGroup>();
            waveStartPanel.SetActive(false);
        }
        
        if (waveCompletePanel != null)
        {
            waveCompleteCanvasGroup = waveCompletePanel.GetComponent<CanvasGroup>();
            if (waveCompleteCanvasGroup == null)
                waveCompleteCanvasGroup = waveCompletePanel.AddComponent<CanvasGroup>();
            waveCompletePanel.SetActive(false);
        }
    }
    
    void Update()
    {
        if (waveManager == null) return;
        
        // Update wave info
        if (waveText != null)
        {
            waveText.text = $"Wave {waveManager.CurrentWave}/{waveManager.TotalWaves}";
        }
        
        // Update enemies remaining
        if (enemiesRemainingText != null && waveManager.IsWaveActive)
        {
            enemiesRemainingText.text = $"Enemies: {waveManager.EnemiesRemaining}";
        }
        else if (enemiesRemainingText != null)
        {
            enemiesRemainingText.text = "";
        }
        
        // Update progress bar
        if (waveProgressBar != null)
        {
            float progress = (float)waveManager.CurrentWave / waveManager.TotalWaves;
            waveProgressBar.value = progress;
        }
    }
    
    void OnWaveStart(int waveNumber)
    {
        if (waveStartPanel != null)
        {
            StartCoroutine(ShowPanel(waveStartPanel, waveStartCanvasGroup, $"Wave {waveNumber} Starting!"));
        }
    }
    
    void OnWaveComplete(int waveNumber)
    {
        if (waveCompletePanel != null)
        {
            StartCoroutine(ShowPanel(waveCompletePanel, waveCompleteCanvasGroup, $"Wave {waveNumber} Complete!"));
        }
    }
    
    void OnAllWavesComplete()
    {
        if (waveCompletePanel != null)
        {
            StartCoroutine(ShowPanel(waveCompletePanel, waveCompleteCanvasGroup, "All Waves Complete!\nVictory!"));
        }
    }
    
    System.Collections.IEnumerator ShowPanel(GameObject panel, CanvasGroup canvasGroup, string message)
    {
        panel.SetActive(true);
        
        // Find text component in panel and set message
        TextMeshProUGUI panelText = panel.GetComponentInChildren<TextMeshProUGUI>();
        if (panelText != null)
        {
            panelText.text = message;
        }
        
        // Fade in animation
        float elapsed = 0f;
        while (elapsed < panelDisplayDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / panelDisplayDuration;
            
            if (progress <= 0.2f)
            {
                // Fade in
                canvasGroup.alpha = fadeInCurve.Evaluate(progress / 0.2f);
            }
            else if (progress >= 0.8f)
            {
                // Fade out
                canvasGroup.alpha = fadeInCurve.Evaluate(1f - ((progress - 0.8f) / 0.2f));
            }
            else
            {
                // Stay visible
                canvasGroup.alpha = 1f;
            }
            
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        panel.SetActive(false);
    }
}
