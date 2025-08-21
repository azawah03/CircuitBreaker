using UnityEngine;

public class EnemyAudioController : MonoBehaviour
{
    [Header("Audio Profile")]
    public EnemyAudioProfile audioProfile;
    
    private EnemyAI enemyAI;
    
    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        
        if (audioProfile != null && enemyAI != null)
        {
            // Apply audio profile settings to EnemyAI
            enemyAI.hitSound = audioProfile.hitSound;
            enemyAI.ambientSound = audioProfile.ambientSound;
            enemyAI.ambientVolume = audioProfile.ambientVolume;
            enemyAI.ambientSoundDelay = audioProfile.ambientDelay;
            enemyAI.randomizePitch = audioProfile.randomizePitch;
        }
    }
    
    public void PlayDeathSound()
    {
        if (audioProfile != null && audioProfile.deathSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(audioProfile.pitchMin, audioProfile.pitchMax);
                audioSource.PlayOneShot(audioProfile.deathSound);
            }
        }
    }
}
