using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement magnitude
        Vector2 movement = new Vector2(horizontal, vertical);
        float speed = movement.magnitude;

        // Update animator
        animator.SetFloat("Speed", speed);

        // Debug
        if (speed > 0)
            Debug.Log("Moving Speed: " + speed);
    }
}