using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;
    private CharacterController characterController;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(horizontal, vertical);
        float speed = movement.magnitude;

        // Adjust speed for sprint
        if (playerMovement.IsSprinting && speed > 0)
        {
            speed = 2.0f; // Higher value triggers sprint animation
        }

        // Update animator
        animator.SetFloat("Speed", speed);

        // Jump animation
        bool isJumping = !characterController.isGrounded;
        animator.SetBool("IsJumping", isJumping);
    }
}