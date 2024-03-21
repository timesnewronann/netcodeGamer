using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables for player controller and movement 
    [SerializeField] private CharacterController controller;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float mouseSensitivity = 100f;

    // Variables for camera movement and checking if player is on the ground
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;

    // Sound effects for moving and jumping
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioSource audioSource;

    private float footstepTimer;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;


    private void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;

        if (!cameraTransform)
            Debug.LogError("Camera Transform is not set on PlayerMovement script.");

        if (!audioSource)
        {
            Debug.LogError("AudioSource is not set on PlayerMovement script.");
        }
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Just a small force to ensure the player stays grounded
        }

        MovePlayer();
        LookAround();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Apply gravity over time (the gravity needs to be applied continuously)
        velocity.y += gravityValue * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        // Call Footsteps method
        if (isGrounded && controller.velocity.magnitude > 0.1f)
        {
            Footsteps();
        }
    }


    private void MovePlayer()
    {
        // Get the input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Determine if sprinting
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        // Move in the direction the player is facing
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    private void Footsteps()
    {
        if (!audioSource.isPlaying)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                audioSource.clip = footstepSound;
                audioSource.Play();
                // Reset the footstepTimer with a delay depending on player's speed 
                footstepTimer = 1f / (controller.velocity.magnitude + 0.1f);
            }
        }
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityValue);
        audioSource.PlayOneShot(jumpSound); // Play jump sound when jumping
    }

    private void LookAround()
    {
        // Get the mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player object around the y-axis
        transform.Rotate(Vector3.up * mouseX);

        // Subtract the vertical input to invert the vertical axis
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the rotation to the camera on the x-axis
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
