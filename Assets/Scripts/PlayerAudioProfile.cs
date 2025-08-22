using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAudioProfile", menuName = "Audio/Player Audio Profile")]
public class PlayerAudioProfile : ScriptableObject
{
    [Header("Footstep Sounds")]
    public AudioClip[] walkFootsteps;
    public AudioClip[] runFootsteps;
    [Range(0f, 1f)]
    public float footstepVolume = 0.7f;
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;
    
    [Header("Movement Sounds")]
    public AudioClip jumpSound;
    public AudioClip landingSound;
    [Range(0f, 1f)]
    public float jumpVolume = 0.8f;
    [Range(0f, 1f)]
    public float landingVolume = 0.9f;
    
    [Header("Combat Sounds")]
    public AudioClip[] hurtSounds;
    public AudioClip deathSound;
    [Range(0f, 1f)]
    public float hurtVolume = 0.8f;
    [Range(0f, 1f)]
    public float deathVolume = 1f;
    
    [Header("Interaction Sounds")]
    public AudioClip pickupSound;
    public AudioClip powerUpSound;
    [Range(0f, 1f)]
    public float interactionVolume = 0.7f;
}
