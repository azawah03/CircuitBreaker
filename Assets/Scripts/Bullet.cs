using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public float speed = 20f;
    public float lifetime = 2f;

    public AudioClip fireSound;
    public AudioClip hitSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = 1.7f;
            Debug.Log("Bullet: Added missing AudioSource");
        }
        
        Debug.Log($"Bullet Start - AudioSource: {audioSource != null}, FireSound: {fireSound != null}, AudioManager: {AudioManager.Instance != null}");
        
        // Play fire sound
        if (fireSound != null && audioSource != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.PlaySFX(audioSource, fireSound);
                Debug.Log("Playing fire sound via AudioManager");
            }
            else
            {
                audioSource.PlayOneShot(fireSound, 1.7f);
                Debug.Log("Playing fire sound directly (no AudioManager)");
            }
        }
        else
        {
            Debug.LogWarning("Cannot play fire sound - missing AudioSource or AudioClip");
        }
        
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            this.enabled = false;

            // Disable the collider so it can't hit anything else.
            GetComponent<Collider>().enabled = false;

            if (GetComponent<MeshRenderer>() != null)
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
            
            // Play hit sound from bullet's audio source
            if (hitSound != null && audioSource != null)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.PlaySFX(audioSource, hitSound);
                    Debug.Log("Playing hit sound via AudioManager");
                }
                else
                {
                    audioSource.PlayOneShot(hitSound, 1.7f);
                    Debug.Log("Playing hit sound directly (no AudioManager)");
                }
            }
            else
            {
                Debug.LogWarning("Cannot play hit sound - missing AudioSource or AudioClip");
            }
            
            // Get enemy hit sound and play it
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null && enemy.hitSound != null && audioSource != null)
            {
                float finalVolume = enemy.hitSoundVolume;
                
                if (AudioManager.Instance != null)
                {
                    // Calculate the final volume that will be applied
                    float totalVolume = AudioManager.Instance.sfxVolume * AudioManager.Instance.masterVolume * enemy.hitSoundVolume;
                    AudioManager.PlaySFX(audioSource, enemy.hitSound, enemy.hitSoundVolume);
                    Debug.Log($"Playing enemy hit sound via AudioManager - Enemy Volume: {enemy.hitSoundVolume}, Final Volume: {totalVolume}");
                }
                else
                {
                    audioSource.PlayOneShot(enemy.hitSound, finalVolume);
                    Debug.Log($"Playing enemy hit sound directly - Volume: {finalVolume}");
                }
            }
            
            // Add score when enemy is destroyed
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(50); // 50 points per enemy
            }
            
            // Destroy enemy first
            Destroy(other.gameObject);
            
            // Delay bullet destruction to let sound finish
            float maxSoundLength = 0f;
            if (hitSound != null) maxSoundLength = Mathf.Max(maxSoundLength, hitSound.length);
            if (enemy != null && enemy.hitSound != null) maxSoundLength = Mathf.Max(maxSoundLength, enemy.hitSound.length);
            
            Destroy(gameObject, maxSoundLength);
        }
    }
}
