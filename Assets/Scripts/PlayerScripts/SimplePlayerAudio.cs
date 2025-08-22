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
        
        audioSource.spatialBlend = 0.5f; // Semi-3D
        audioSource.volume = 1f;
        audioSource.playOnAwake = false;
        
        wasGroundedLastFrame = controller.isGrounded;
        
        Debug.Log($"SimplePlayerAudio Start - Components found: PlayerMovement({playerMovement != null}), CharacterController({controller != null}), AudioSource({audioSource != null})");
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
        
        if (currentFootsteps != null && currentFootsteps.Length > 0 && audioSource != null)
        {
            AudioClip randomFootstep = currentFootsteps[Random.Range(0, currentFootsteps.Length)];
            
            if (randomFootstep != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(randomFootstep, footstepVolume);
                Debug.Log($"Played footstep: {randomFootstep.name} (Volume: {footstepVolume})");
            }
        }
        else
        {
            Debug.Log("No footstep clips assigned or missing audio source!");
        }
    }
    
    void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(jumpSound, jumpVolume);
            Debug.Log($"Played jump sound: {jumpSound.name}");
        }
    }
    
    void PlayLandingSound()
    {
        if (landingSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(landingSound, landingVolume);
            Debug.Log($"Played landing sound: {landingSound.name}");
        }
    }
}
