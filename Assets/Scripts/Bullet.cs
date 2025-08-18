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
        Debug.Log("Bullet Start - AudioSource: " + (audioSource != null) + ", FireSound: " + (fireSound != null));
        
        if (fireSound != null && audioSource != null)
        {
            AudioManager.PlaySFX(audioSource, fireSound);
            Debug.Log("Playing fire sound");
        }
        else
        {
            Debug.Log("Cannot play fire sound - missing AudioSource or AudioClip");
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
            Debug.Log("Bullet hit enemy");
            
            // Play hit sound from bullet's audio source
            if (hitSound != null && audioSource != null)
            {
                AudioManager.PlaySFX(audioSource, hitSound);
                Debug.Log("Playing hit sound");
            }
            else
            {
                Debug.Log("Cannot play hit sound - missing AudioSource or AudioClip");
            }
            
            // Get enemy hit sound and play it from bullet's audio source
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null && enemy.hitSound != null && audioSource != null)
            {
                AudioManager.PlaySFX(audioSource, enemy.hitSound);
                Debug.Log("Playing enemy hit sound from bullet");
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
