using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float boostMultiplier = 2f;
    public float lookSensitivity = 2f;
    public float scrollSensitivity = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Look rotation
        rotationX += Input.GetAxis("Mouse X") * lookSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);
        transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);

        // Movement input
        Vector3 direction = new Vector3(
            Input.GetAxis("Horizontal"),
            (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.LeftShift) ? 1 : 0),
            Input.GetAxis("Vertical")
        );

        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftControl) ? boostMultiplier : 1f);
        transform.position += transform.TransformDirection(direction.normalized) * speed * Time.deltaTime;

        // Scroll to adjust base move speed
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            moveSpeed += scroll * scrollSensitivity;
            moveSpeed = Mathf.Max(0.1f, moveSpeed); // prevent going negative
        }

        // Unlock cursor toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}