using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f; // Time between shots

    private float nextFireTime = 0f;
    private Animator animator; 

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            animator = GetComponentInParent<Animator>();
        }
    }

    void Update()
    {
        // Only allow shooting when game is in Playing state
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Playing)
            return;

        // Check if the left mouse button is being held down.
        bool isAiming = Input.GetMouseButton(0);

        // Set the "IsAiming" parameter in the Animator Controller.
        if (animator != null)
        {
            animator.SetBool("IsAiming", isAiming);
        }

        if (isAiming && Time.time >= nextFireTime)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + fireRate;
        }
    }
}