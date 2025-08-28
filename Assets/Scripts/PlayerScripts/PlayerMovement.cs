using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public float moveSpeed = 7f;
    public float gravity = -10f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 6f;
    public float sprintSpeed = 12f;
    public float stamina = 5f;
    public float maxStamina = 5f;
    public float staminaRegenRate = 2f;
    public float staminaDrainRate = 1f;
    public float staminaCooldownDuration = 2f;
    private float staminaCooldownTimer = 0f;
    private float currentSpeed;
    public bool IsSprinting { get; private set; }
    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = moveSpeed;
        IsSprinting = false;
        stamina = maxStamina;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical);
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);

        // Handle cooldown timing
        if (stamina <= 0f)
        {
            staminaCooldownTimer = staminaCooldownDuration;
        }
        if (staminaCooldownTimer > 0f)
        {
            staminaCooldownTimer -= Time.deltaTime;
        }

        // Sprint logic with stamina
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift);
        bool wantsToSprint = shiftHeld && inputDir.sqrMagnitude > 0.01f;
        bool canSprint = stamina > 0f && staminaCooldownTimer <= 0f;

        if (wantsToSprint && canSprint)
        {
            IsSprinting = true;
            currentSpeed = sprintSpeed;
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Max(stamina, 0f);
        }
        else
        {
            IsSprinting = false;
            currentSpeed = moveSpeed;
            if (!shiftHeld || inputDir.sqrMagnitude < 0.01f)
            {
                stamina += staminaRegenRate * Time.deltaTime;
                stamina = Mathf.Min(stamina, maxStamina);
            }
        }

        // Jumping
        bool isGrounded = controller.isGrounded || velocity.y < 0.1f && velocity.y > -0.1f;
        if (isGrounded)
        {
            velocity.y = -2f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = jumpHeight;
            }
        }
        velocity.y += gravity * Time.deltaTime;

        // Movement
        if (inputDir.sqrMagnitude > 0.01f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;
            moveDir.Normalize();

            // Only apply movement if grounded
            if (controller.isGrounded)
            {
                controller.Move(moveDir * currentSpeed * Time.deltaTime);
            }
            else
            {
                controller.Move(moveDir * currentSpeed * 0.3f * Time.deltaTime);
            }

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            lastMoveDirection = moveDir;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    public Vector3 lastMoveDirection { get; private set; } = Vector3.forward;
}