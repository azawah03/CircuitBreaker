using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Background Music")]
    public AudioClip backgroundMusic;
    public float volume = 0.5f;
    [Range(0f, 1f)]
    public float fadeInDuration = 2f;
    
    private AudioSource audioSource;
    
    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure AudioSource for background music
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f; // Start at 0 for fade in effect
        
        // Play background music
        if (backgroundMusic != null)
        {
            audioSource.Play();
            StartCoroutine(FadeIn());
        }
        else
        {
            Debug.LogWarning("No background music assigned to BackgroundMusicManager!");
        }
    }
    
    private System.Collections.IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        float startVolume = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, volume, elapsedTime / fadeInDuration);
            yield return null;
        }
        
        audioSource.volume = volume;
    }
    
    // Public methods to control background music
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
            audioSource.volume = volume;
    }
    
    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Pause();
    }
    
    public void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.UnPause();
    }
    
    public void StopMusic()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
