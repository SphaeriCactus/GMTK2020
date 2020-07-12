using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public bool lockMovement = false;
    public Edward edward;
    public float mouseSensitivity = 100f;

    private Transform playerTransform;

    private float xRotation;

    private bool hasLooked;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerTransform = transform.parent;
        hasLooked = false;
    }

    void FixedUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (lockMovement)
        {
            if ((mouseX > 0.1f || mouseY > 0.1f) && !hasLooked)
            {
                hasLooked = true;
                edward.Speak(1);
            }
        }

        playerTransform.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
