using UnityEngine;

public class SimplePlayerAudio : MonoBehaviour
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
    
    private PlayerMovement playerMovement;
    private CharacterController controller;
    private AudioSource audioSource;
    private AudioSource[] footstepAudioSources; // Multiple audio sources for clean footsteps
    private int currentFootstepSource = 0;
    
    private bool wasGroundedLastFrame;
    private float footstepTimer;
    private bool hasPlayedJumpSound;
    
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Create multiple AudioSources for footsteps to avoid any interference
        footstepAudioSources = new AudioSource[3]; // Use 3 sources to prevent overlap issues
        for (int i = 0; i < footstepAudioSources.Length; i++)
        {
            GameObject footstepObj = new GameObject($"FootstepAudio_{i}");
            footstepObj.transform.SetParent(transform);
            footstepObj.transform.localPosition = Vector3.zero;
            footstepAudioSources[i] = footstepObj.AddComponent<AudioSource>();
            footstepAudioSources[i].spatialBlend = 0.5f; // Semi-3D
            footstepAudioSources[i].volume = 1f;
            footstepAudioSources[i].pitch = 1f; // Always keep pitch at 1
            footstepAudioSources[i].playOnAwake = false;
        }

        audioSource.spatialBlend = 0.5f; // Semi-3D
        audioSource.volume = 1f;
        audioSource.pitch = 1f; // Always keep pitch at 1
        audioSource.playOnAwake = false;

        wasGroundedLastFrame = controller.isGrounded;
        
        Debug.Log($"SimplePlayerAudio Start - Components found: PlayerMovement({playerMovement != null}), CharacterController({controller != null}), AudioSource({audioSource != null}), FootstepAudioSources({footstepAudioSources.Length})");
        Debug.Log($"Audio clips - Walk: {walkFootsteps?.Length ?? 0}, Run: {runFootsteps?.Length ?? 0}, Jump: {jumpSound != null}, Landing: {landingSound != null}");
    }
    
    void Update()
    {
        HandleFootsteps();
        HandleJumpAndLanding();
    }
    
    void HandleFootsteps()
    {
        if (controller == null || playerMovement == null) return;
        
        bool isGrounded = controller.isGrounded;
        bool isMoving = controller.velocity.magnitude > 0.1f;
        
        if (isGrounded && isMoving)
        {
            footstepTimer += Time.deltaTime;
            
            float currentStepInterval = playerMovement.IsSprinting ? runStepInterval : walkStepInterval;
            
            if (footstepTimer >= currentStepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }
    
    void HandleJumpAndLanding()
    {
        if (controller == null) return;
        
        bool isGrounded = controller.isGrounded;
        
        // Jump sound
        if (!isGrounded && wasGroundedLastFrame && !hasPlayedJumpSound)
        {
            PlayJumpSound();
            hasPlayedJumpSound = true;
        }
        
        // Landing sound
        if (isGrounded && !wasGroundedLastFrame)
        {
            PlayLandingSound();
            hasPlayedJumpSound = false;
        }
        
        wasGroundedLastFrame = isGrounded;
    }
    
    void PlayFootstepSound()
    {
        AudioClip[] currentFootsteps = playerMovement.IsSprinting ? runFootsteps : walkFootsteps;
        
        if (currentFootsteps != null && currentFootsteps.Length > 0 && footstepAudioSources != null)
        {
            AudioClip randomFootstep = currentFootsteps[Random.Range(0, currentFootsteps.Length)];
            
            if (randomFootstep != null)
            {
                // Use round-robin approach to get next available audio source
                AudioSource currentSource = footstepAudioSources[currentFootstepSource];
                currentFootstepSource = (currentFootstepSource + 1) % footstepAudioSources.Length;
                
                // Play with NO pitch modification - just clean audio
                currentSource.clip = randomFootstep;
                currentSource.volume = footstepVolume;
                currentSource.pitch = 1f; // Always 1, no variation
                currentSource.Play();
                
                Debug.Log($"Played clean footstep: {randomFootstep.name} on source {currentFootstepSource - 1}");
            }
        }
        else
        {
            Debug.Log("No footstep clips assigned or missing footstep audio sources!");
        }
    }
    
    void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            // Play with NO pitch modification - just clean audio
            audioSource.clip = jumpSound;
            audioSource.volume = jumpVolume;
            audioSource.pitch = 1f; // Always 1, no variation
            audioSource.Play();
            Debug.Log($"Played clean jump sound: {jumpSound.name}");
        }
    }
    
    void PlayLandingSound()
    {
        if (landingSound != null && audioSource != null)
        {
            // Play with NO pitch modification - just clean audio
            audioSource.clip = landingSound;
            audioSource.volume = landingVolume;
            audioSource.pitch = 1f; // Always 1, no variation
            audioSource.Play();
            Debug.Log($"Played clean landing sound: {landingSound.name}");
        }
    }
}
