using UnityEngine;

public class SimplePlayerAudio : MonoBehaviour
{
    [Header("Footstep Sounds")]
    public AudioClip[] walkFootsteps;
    public AudioClip[] runFootsteps;
    [Range(0f, 1f)]
    public float footstepVolume = 0.7f;
    public float walkStepInterval = 0.6f;  // Slightly faster for better sync
    public float runStepInterval = 0.35f;  // Faster for running
    
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
    private float lastFootstepTime; // Prevent rapid-fire footsteps
    
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
        
        // Get existing AudioSource or create a clean one
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Reset AudioSource to clean defaults
        audioSource.clip = null;
        audioSource.volume = 1f;
        audioSource.pitch = 1f;
        audioSource.spatialBlend = 0f; // 2D sound to avoid 3D audio issues
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.priority = 128;
        audioSource.panStereo = 0f;
        audioSource.reverbZoneMix = 1f;
        audioSource.dopplerLevel = 1f;
        audioSource.spread = 0f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 500f;

        wasGroundedLastFrame = controller.isGrounded;
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
        
        // Use input detection instead of velocity (more reliable for CharacterController)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isMoving = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f);
        
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
        // Prevent playing footsteps too rapidly (min 0.1 seconds between steps)
        if (Time.time - lastFootstepTime < 0.1f) return;
        
        AudioClip[] currentFootsteps = playerMovement.IsSprinting ? runFootsteps : walkFootsteps;
        
        if (currentFootsteps != null && currentFootsteps.Length > 0 && audioSource != null)
        {
            AudioClip randomFootstep = currentFootsteps[Random.Range(0, currentFootsteps.Length)];
            
            if (randomFootstep != null)
            {
                // Use PlayOneShot without stopping - let sounds overlap naturally
                audioSource.PlayOneShot(randomFootstep, footstepVolume);
                lastFootstepTime = Time.time;
            }
        }
    }
    
    void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpSound, jumpVolume);
        }
    }
    
    void PlayLandingSound()
    {
        if (landingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(landingSound, landingVolume);
        }
    }
}
