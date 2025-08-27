using UnityEngine;

public class AmbientSoundTester : MonoBehaviour
{
    [Header("Ambient Sound Testing")]
    public AudioClip testAmbientSound;
    [Range(0f, 2f)]
    public float testVolume = 1f;
    public KeyCode toggleKey = KeyCode.T;
    public KeyCode volumeUpKey = KeyCode.Plus;
    public KeyCode volumeDownKey = KeyCode.Minus;
    
    private AudioSource testAmbientSource;
    private bool isPlaying = false;
    
    void Start()
    {
        // Create ambient sound source
        GameObject ambientObj = new GameObject("AmbientTester");
        ambientObj.transform.SetParent(transform);
        testAmbientSource = ambientObj.AddComponent<AudioSource>();
        
        if (testAmbientSound != null)
        {
            testAmbientSource.clip = testAmbientSound;
            testAmbientSource.loop = true;
            testAmbientSource.volume = testVolume;
            testAmbientSource.spatialBlend = 0.7f;
            testAmbientSource.minDistance = 3f;
            testAmbientSource.maxDistance = 25f;
            testAmbientSource.priority = 96;
            testAmbientSource.playOnAwake = false;
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleAmbientSound();
        }
        
        if (Input.GetKeyDown(volumeUpKey))
        {
            testVolume = Mathf.Min(testVolume + 0.1f, 2f);
            UpdateVolume();
        }
        
        if (Input.GetKeyDown(volumeDownKey))
        {
            testVolume = Mathf.Max(testVolume - 0.1f, 0f);
            UpdateVolume();
        }
    }
    
    void ToggleAmbientSound()
    {
        if (testAmbientSource != null && testAmbientSound != null)
        {
            if (isPlaying)
            {
                testAmbientSource.Stop();
                isPlaying = false;
                Debug.Log("Stopped ambient sound");
            }
            else
            {
                UpdateVolume();
                testAmbientSource.Play();
                isPlaying = true;
                Debug.Log($"Started ambient sound at volume: {testAmbientSource.volume}");
            }
        }
    }
    
    void UpdateVolume()
    {
        if (testAmbientSource != null)
        {
            float finalVolume = testVolume;
            if (AudioManager.Instance != null)
            {
                finalVolume *= AudioManager.Instance.sfxVolume * AudioManager.Instance.masterVolume;
            }
            testAmbientSource.volume = finalVolume;
            Debug.Log($"Updated ambient volume to: {finalVolume} (base: {testVolume})");
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(320, 10, 300, 150));
        GUILayout.Label("Ambient Sound Tester");
        GUILayout.Label($"Press {toggleKey} to toggle ambient sound");
        GUILayout.Label($"Press {volumeUpKey}/{volumeDownKey} to adjust volume");
        GUILayout.Label($"Current Volume: {testVolume:F2}");
        GUILayout.Label($"Is Playing: {isPlaying}");
        
        if (testAmbientSource != null)
        {
            GUILayout.Label($"Actual Volume: {testAmbientSource.volume:F2}");
        }
        
        GUILayout.EndArea();
    }
}
