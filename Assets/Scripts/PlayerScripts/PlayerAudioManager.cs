using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Footstep Sounds")]
    public AudioClip[] walkFootsteps;
    public AudioClip[] runFootsteps;
    [Range(0f, 1f)]
    public float footstepVolume = 0.7f;
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;
    
    [Header("Jump/Landing Sounds")]
    public AudioClip jumpSound;
    public AudioClip landingSound;
    [Range(0f, 1f)]
    public float jumpVolume = 0.8f;
    [Range(0f, 1f)]
    public float landingVolume = 0.9f;
    
    [Header("Audio Sources")]
    public AudioSource footstepAudioSource;
    public AudioSource jumpAudioSource;
    
    private PlayerMovement playerMovement;
    private CharacterController controller;
    private bool wasGroundedLastFrame;
    private bool wasMovingLastFrame;
    private float footstepTimer;
    private bool hasPlayedJumpSound;
    
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
        
        Debug.Log($"PlayerAudioManager Start - PlayerMovement: {playerMovement != null}, CharacterController: {controller != null}");
        
        // Create audio sources if not assigned
        if (footstepAudioSource == null)
        {
            GameObject footstepObj = new GameObject("FootstepAudio");
            footstepObj.transform.SetParent(transform);
            footstepObj.transform.localPosition = Vector3.zero;
            footstepAudioSource = footstepObj.AddComponent<AudioSource>();
            Debug.Log("Created footstep audio source");
        }
        
        if (jumpAudioSource == null)
        {
            GameObject jumpObj = new GameObject("JumpAudio");
            jumpObj.transform.SetParent(transform);
            jumpObj.transform.localPosition = Vector3.zero;
            jumpAudioSource = jumpObj.AddComponent<AudioSource>();
            Debug.Log("Created jump audio source");
        }
        
        // Configure audio sources
        SetupAudioSource(footstepAudioSource, 64); // Medium priority
        SetupAudioSource(jumpAudioSource, 32); // High priority
        
        wasGroundedLastFrame = controller.isGrounded;
        
        Debug.Log($"Audio Setup Complete - Walk clips: {walkFootsteps?.Length ?? 0}, Run clips: {runFootsteps?.Length ?? 0}");
    }
    
    void SetupAudioSource(AudioSource source, int priority)
    {
        source.spatialBlend = 0.8f; // Mostly 3D
        source.minDistance = 1f;
        source.maxDistance = 10f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.priority = priority;
        source.playOnAwake = false;
    }
    
    void Update()
    {
        HandleFootsteps();
        HandleJumpAndLanding();
    }
    
    void HandleFootsteps()
    {
        bool isGrounded = controller.isGrounded;
        bool isMoving = controller.velocity.magnitude > 0.1f;
        
        if (isGrounded && isMoving)
        {
            footstepTimer += Time.deltaTime;
            
            // Determine step interval based on sprinting
            float currentStepInterval = playerMovement.IsSprinting ? runStepInterval : walkStepInterval;
            
            if (footstepTimer >= currentStepInterval)
            {
                Debug.Log($"Playing footstep - Grounded: {isGrounded}, Moving: {isMoving}, Sprinting: {playerMovement.IsSprinting}");
                PlayFootstepSound();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
        
        wasMovingLastFrame = isMoving;
    }
    
    void HandleJumpAndLanding()
    {
        bool isGrounded = controller.isGrounded;
        
        // Jump sound
        if (!isGrounded && wasGroundedLastFrame && !hasPlayedJumpSound)
        {
            Debug.Log("Player jumped - playing jump sound");
            PlayJumpSound();
            hasPlayedJumpSound = true;
        }
        
        // Landing sound
        if (isGrounded && !wasGroundedLastFrame)
        {
            Debug.Log("Player landed - playing landing sound");
            PlayLandingSound();
            hasPlayedJumpSound = false;
        }
        
        wasGroundedLastFrame = isGrounded;
    }
    
    void PlayFootstepSound()
    {
        AudioClip[] currentFootsteps = playerMovement.IsSprinting ? runFootsteps : walkFootsteps;
        
        Debug.Log($"PlayFootstepSound - Using {(playerMovement.IsSprinting ? "run" : "walk")} footsteps, Array length: {currentFootsteps?.Length ?? 0}");
        
        if (currentFootsteps != null && currentFootsteps.Length > 0)
        {
            AudioClip randomFootstep = currentFootsteps[Random.Range(0, currentFootsteps.Length)];
            
            if (randomFootstep != null && footstepAudioSource != null)
            {
                // Add slight pitch variation for more natural sound
                footstepAudioSource.pitch = Random.Range(0.9f, 1.1f);
                AudioManager.PlaySFX(footstepAudioSource, randomFootstep, footstepVolume);
                Debug.Log($"Played footstep: {randomFootstep.name}");
            }
            else
            {
                Debug.Log($"Cannot play footstep - Clip: {randomFootstep != null}, AudioSource: {footstepAudioSource != null}");
            }
        }
        else
        {
            Debug.Log("No footstep clips assigned!");
        }
    }
    
    void PlayJumpSound()
    {
        if (jumpSound != null && jumpAudioSource != null)
        {
            jumpAudioSource.pitch = Random.Range(0.95f, 1.05f);
            AudioManager.PlaySFX(jumpAudioSource, jumpSound, jumpVolume);
            Debug.Log($"Played jump sound: {jumpSound.name}");
        }
        else
        {
            Debug.Log($"Cannot play jump sound - Clip: {jumpSound != null}, AudioSource: {jumpAudioSource != null}");
        }
    }
    
    void PlayLandingSound()
    {
        if (landingSound != null && jumpAudioSource != null)
        {
            jumpAudioSource.pitch = Random.Range(0.9f, 1.1f);
            AudioManager.PlaySFX(jumpAudioSource, landingSound, landingVolume);
            Debug.Log($"Played landing sound: {landingSound.name}");
        }
        else
        {
            Debug.Log($"Cannot play landing sound - Clip: {landingSound != null}, AudioSource: {jumpAudioSource != null}");
        }
    }
    
    // Public methods for external control
    public void SetFootstepVolume(float volume)
    {
        footstepVolume = Mathf.Clamp01(volume);
    }
    
    public void SetJumpVolume(float volume)
    {
        jumpVolume = Mathf.Clamp01(volume);
        landingVolume = Mathf.Clamp01(volume);
    }
    
    public void MuteFootsteps(bool mute)
    {
        if (footstepAudioSource != null)
        {
            footstepAudioSource.mute = mute;
        }
    }
}
