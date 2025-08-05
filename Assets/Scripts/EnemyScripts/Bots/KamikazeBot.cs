using UnityEngine;

public class KamikazeBot : EnemyAI
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private GameObject explosionEffect;

    public void Setup(Transform playerTarget, float speed, float damage, GameObject explosionFX)
    {
        target = playerTarget;
        moveSpeed = speed;
        explosionDamage = damage;
        explosionEffect = explosionFX;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(explosionDamage); 
            }

            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    protected override void UpdateBehavior()
    {
        // Kamikaze just moves toward player — no extra logic needed
    }
}
