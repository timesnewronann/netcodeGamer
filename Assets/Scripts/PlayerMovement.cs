using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float mouseSensitivity = 100f;
    
    private float xRotation = 0f; // For vertical camera rotation
    private Camera playerCamera;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        // If the player has its own camera that's not the main scene camera, 
        // find it like this (adjust the path to where the camera is in the hierarchy if necessary):
        playerCamera = GetComponentInChildren<Camera>();

        // Fallback, check if we found a camera, otherwise try to get the main camera
        if (!playerCamera) {
            Debug.LogWarning("Player camera not found in children, using main camera.");
            playerCamera = Camera.main;
        }

        // Check if we successfully found a camera
        if (!playerCamera) {
            Debug.LogError("No player camera assigned and no main camera found.");
        }
    }


    private void Update() {
        // Movement input
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

        // Mouse look functionality
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the rotation to the player's camera and character
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    
}
