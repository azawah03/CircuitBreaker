using UnityEngine;

public class PowerUpVisual : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float floatSpeed = 1f;
    public float floatHeight = 0.5f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Rotate
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Float up and down
        float newY = startPos.y + (Mathf.Sin(Time.time * floatSpeed) * floatHeight);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}