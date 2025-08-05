using System.Diagnostics;
using UnityEngine;

public class RangedBot : EnemyAI
{
    [Header("Ranged Attack Settings")]
    public Transform firePoint;
    public float attackRange = 15f;        // Ideal distance to maintain
    public float retreatDistance = 7f;     // If too close, move back
    public float fireRate = 2f;
    public float projectileSpeed = 20f;
    public float projectileDamage = 10f;

    private float fireTimer;

    protected override void UpdateBehavior()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // Always look at player
        Vector3 lookDir = (target.position - transform.position).normalized;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);

        // Movement logic: maintain attack range
        if (distance > attackRange)
        {
            MoveTowards(target.position); // move closer
        }
        else if (distance < retreatDistance)
        {
            Vector3 retreatDir = (transform.position - target.position).normalized;
            MoveTowards(transform.position + retreatDir); // move away
        }

        // Fire if within range
        fireTimer += Time.deltaTime;
        if (distance <= attackRange && fireTimer >= fireRate)
        {
            FireLaser();
            fireTimer = 0f;
        }
    }

    private void FireLaser()
    {
        if (firePoint == null)
        {
            UnityEngine.Debug.LogWarning("RangedBot: No firePoint assigned.");
            return;
        }

        // Root laser object
        GameObject laser = new GameObject("LaserProjectile");
        laser.transform.position = firePoint.position;
        laser.transform.rotation = firePoint.rotation;

        // Visual: cylinder child
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        visual.transform.SetParent(laser.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Face forward
        visual.transform.localScale = new Vector3(0.1f, 0.5f, 0.1f);

        // Collider and Rigidbody on root
        SphereCollider col = laser.AddComponent<SphereCollider>();
        col.isTrigger = true;

        Rigidbody rb = laser.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Add script
        Laser lp = laser.AddComponent<Laser>();
        lp.damage = projectileDamage;
        lp.speed = projectileSpeed;

        // Color
        Renderer rend = visual.GetComponent<Renderer>();
        if (rend != null) rend.material.color = Color.red;

    }
}
