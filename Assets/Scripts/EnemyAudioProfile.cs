using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAudioProfile", menuName = "Audio/Enemy Audio Profile")]
public class EnemyAudioProfile : ScriptableObject
{
    [Header("Enemy Sounds")]
    public AudioClip hitSound;
    public AudioClip ambientSound;
    public AudioClip deathSound;
    
    [Header("Ambient Settings")]
    [Range(0f, 1f)]
    public float ambientVolume = 0.3f;
    public float ambientDelay = 2f;
    public bool randomizePitch = true;
    
    [Header("Voice Settings")]
    [Range(0.5f, 2f)]
    public float pitchMin = 0.8f;
    [Range(0.5f, 2f)]
    public float pitchMax = 1.2f;
}
