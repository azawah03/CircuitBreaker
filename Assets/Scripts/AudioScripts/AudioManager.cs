using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 0.15f; // Even lower music volume
    [Range(0f, 1f)]
    public float sfxVolume = 0.9f; // Higher SFX volume for ambient sounds
    
    private BackgroundMusicManager musicManager;
    
    public static AudioManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton pattern - only one AudioManager should exist
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        musicManager = GetComponent<BackgroundMusicManager>();
    }
    
    void Start()
    {
        // Apply initial settings
        UpdateAudioSettings();
    }
    
    public void UpdateAudioSettings()
    {
        // Don't use AudioListener.volume as it affects everything globally
        // Instead, control volumes individually
        
        // Update music volume
        if (musicManager != null)
        {
            musicManager.SetVolume(musicVolume * masterVolume);
        }
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAudioSettings();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateAudioSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        // SFX volume will be applied when playing individual sound effects
    }
    
    // Helper method for playing SFX with proper volume
    public static void PlaySFX(AudioSource audioSource, AudioClip clip)
    {
        if (Instance != null && audioSource != null && clip != null)
        {
            // Set SFX priority higher than background music
            audioSource.priority = 32; // Lower number = higher priority
            audioSource.PlayOneShot(clip, Instance.sfxVolume * Instance.masterVolume);
        }
    }
    
    // Overloaded method with custom volume
    public static void PlaySFX(AudioSource audioSource, AudioClip clip, float volumeMultiplier)
    {
        if (Instance != null && audioSource != null && clip != null)
        {
            audioSource.priority = 32;
            audioSource.PlayOneShot(clip, Instance.sfxVolume * Instance.masterVolume * volumeMultiplier);
        }
    }
}
