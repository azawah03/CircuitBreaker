using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public PlayerMovement movementScript;
    public float rotationSpeed = 10f;

    void Update()
    {
        Vector3 direction = movementScript.lastMoveDirection;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
