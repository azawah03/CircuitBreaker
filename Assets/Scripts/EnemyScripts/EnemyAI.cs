using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Movement")]
    public float moveSpeed = 3f;
    [SerializeField] protected float rotationSpeed = 5f;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip ambientSound;
    [Range(0f, 2f)] // Allow volumes above 1 for ambient sounds
    public float ambientVolume = 0.8f; // Much higher default volume
    public float ambientSoundDelay = 2f; // Delay before starting ambient sound
    public bool randomizePitch = true;
    private AudioSource audioSource;
    private AudioSource ambientAudioSource;

    protected virtual void Start()
    {
        SetupAudio();
    }

    void SetupAudio()
    {
        // Setup main audio source for hit sounds
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Setup second audio source for ambient sounds
        if (ambientSound != null)
        {
            // Create a child GameObject for ambient sound
            GameObject ambientSoundObj = new GameObject("AmbientSound");
            ambientSoundObj.transform.SetParent(transform);
            ambientSoundObj.transform.localPosition = Vector3.zero;

            ambientAudioSource = ambientSoundObj.AddComponent<AudioSource>();
            ambientAudioSource.clip = ambientSound;
            ambientAudioSource.loop = true;
            ambientAudioSource.volume = ambientVolume;
            ambientAudioSource.spatialBlend = 0.7f; // Less 3D, more audible
            ambientAudioSource.minDistance = 3f; // Larger min distance
            ambientAudioSource.maxDistance = 25f; // Larger max distance
            ambientAudioSource.rolloffMode = AudioRolloffMode.Linear;
            ambientAudioSource.priority = 96; // Lower priority than SFX but higher than music
            ambientAudioSource.playOnAwake = false;

            // Randomize pitch for variety
            if (randomizePitch)
            {
                ambientAudioSource.pitch = Random.Range(0.8f, 1.2f);
            }

            // Start playing ambient sound after delay
            Invoke("StartAmbientSound", ambientSoundDelay + Random.Range(0f, 2f));
        }
    }

    void StartAmbientSound()
    {
        if (ambientAudioSource != null && ambientSound != null)
        {
            // Apply AudioManager volume settings if available
            float finalVolume = ambientVolume;
            if (AudioManager.Instance != null)
            {
                finalVolume *= AudioManager.Instance.sfxVolume * AudioManager.Instance.masterVolume;
            }
            
            ambientAudioSource.volume = finalVolume;
            ambientAudioSource.Play();
            Debug.Log($"Started ambient sound for {gameObject.name} at volume: {finalVolume}");
        }
    }
    
    public void SetAmbientVolume(float newVolume)
    {
        ambientVolume = newVolume;
        if (ambientAudioSource != null)
        {
            float finalVolume = ambientVolume;
            if (AudioManager.Instance != null)
            {
                finalVolume *= AudioManager.Instance.sfxVolume * AudioManager.Instance.masterVolume;
            }
            ambientAudioSource.volume = finalVolume;
        }
    }

    protected virtual void Update()
    {
        if (target == null) return;

        // Move toward target
        Vector3 direction = (target.position - transform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            // Rotate toward target 
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move forward
            Vector3 moveDir = direction.normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        UpdateBehavior(); // Let subclasses override their own logic
    }

    protected void MoveTowards(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        direction.y = 0f;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    public void PlayHitSound()
    {
        Debug.Log("Enemy PlayHitSound called - HitSound: " + (hitSound != null));
        
        if (hitSound != null)
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
                
            Debug.Log("Enemy AudioSource: " + (audioSource != null));
            
            if (audioSource != null)
            {
                // Ensure AudioSource is enabled
                audioSource.enabled = true;
                audioSource.PlayOneShot(hitSound);
                Debug.Log("Playing enemy hit sound");
            }
        }
    }

    public void StopAmbientSound()
    {
        if (ambientAudioSource != null)
        {
            ambientAudioSource.Stop();
        }
    }

    public void PauseAmbientSound()
    {
        if (ambientAudioSource != null && ambientAudioSource.isPlaying)
        {
            ambientAudioSource.Pause();
        }
    }

    public void ResumeAmbientSound()
    {
        if (ambientAudioSource != null && !ambientAudioSource.isPlaying)
        {
            ambientAudioSource.UnPause();
        }
    }

    protected virtual void OnDestroy()
    {
        // Stop ambient sound when enemy is destroyed
        StopAmbientSound();
        
        // Cancel any pending ambient sound start
        CancelInvoke("StartAmbientSound");
    }


    // Hook for child classes
    protected virtual void UpdateBehavior() { }
}
