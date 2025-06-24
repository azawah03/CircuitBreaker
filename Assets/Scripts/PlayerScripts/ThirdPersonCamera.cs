using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2.5f, -5f);
    public float sensitivity = 3f;
    public float pitchMin = 0f;     // Prevent looking down below horizon
    public float pitchMax = 60f;
    public float collisionBuffer = 0.3f;
    public LayerMask collisionLayers; // Set to "Default" or your ground layer

    private float yaw = 0f;
    private float pitch = 10f;
    private Transform cam;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Rotate camera with mouse
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredCameraPos = target.position + rotation * offset;

        // Prevent clipping through geometry using SphereCast
        Vector3 direction = desiredCameraPos - target.position;
        float distance = direction.magnitude;
        Vector3 origin = target.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(origin, 0.2f, direction.normalized, out RaycastHit hit, distance + collisionBuffer, collisionLayers))
        {
            desiredCameraPos = hit.point - direction.normalized * collisionBuffer;
        }

        // Apply position and rotation
        transform.position = desiredCameraPos;

        // Hard limit: don't go under the floor (e.g., under Y = 1)
        if (transform.position.y < 1f)
        {
            Vector3 pos = transform.position;
            pos.y = 1f;
            transform.position = pos;
        }

        transform.rotation = rotation;
        cam.LookAt(target.position + Vector3.up * 1.5f);
    }
}
