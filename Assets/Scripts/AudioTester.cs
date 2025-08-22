using UnityEngine;

public class AudioTester : MonoBehaviour
{
    [Header("Volume Testing")]
    [Range(0f, 1f)]
    public float testMasterVolume = 1f;
    [Range(0f, 1f)]
    public float testMusicVolume = 0.2f;
    [Range(0f, 1f)]
    public float testSFXVolume = 0.8f;
    
    [Header("Test Audio")]
    public AudioClip testSFX;
    public KeyCode testSFXKey = KeyCode.Space;
    
    private AudioSource testAudioSource;
    
    void Start()
    {
        // Create audio source for testing
        testAudioSource = gameObject.AddComponent<AudioSource>();
        testAudioSource.playOnAwake = false;
    }
    
    void Update()
    {
        // Apply volume changes in real-time
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(testMasterVolume);
            AudioManager.Instance.SetMusicVolume(testMusicVolume);
            AudioManager.Instance.SetSFXVolume(testSFXVolume);
        }
        
        // Test SFX with key press
        if (Input.GetKeyDown(testSFXKey) && testSFX != null)
        {
            AudioManager.PlaySFX(testAudioSource, testSFX);
        }
    }
    
    void OnGUI()
    {
        if (AudioManager.Instance != null)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("Audio Volume Controls");
            
            GUILayout.Label($"Master Volume: {testMasterVolume:F2}");
            testMasterVolume = GUILayout.HorizontalSlider(testMasterVolume, 0f, 1f);
            
            GUILayout.Label($"Music Volume: {testMusicVolume:F2}");
            testMusicVolume = GUILayout.HorizontalSlider(testMusicVolume, 0f, 1f);
            
            GUILayout.Label($"SFX Volume: {testSFXVolume:F2}");
            testSFXVolume = GUILayout.HorizontalSlider(testSFXVolume, 0f, 1f);
            
            GUILayout.Label($"Press {testSFXKey} to test SFX");
            
            GUILayout.EndArea();
        }
    }
}
