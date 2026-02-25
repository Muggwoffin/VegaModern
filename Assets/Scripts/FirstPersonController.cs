using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;
    public float moveSpeed = 0f;
    public float mouseSensitivity =20f;
    private Vector3 velocity;
    public float gravity = -9.8f;
    private Vector2 moveInput;
    private Vector2 lookInput;
    public float jumpHeight = 1.0f;
    private bool jumpPressed;
    
    private float xRotation = 0f;
    private PlayerInput playerInput;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {

        MoveCharacter();
        RotationLogic();

    }
    

    private void MoveCharacter()
    {
        // 1. Calculate Horizontal Movement (WASD)
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // 2. Handle Gravity & Grounding
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Snaps player to floor
        }

        // 3. Handle Jumping
        if (controller.isGrounded && jumpPressed)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false; 
        }

        // 4. Apply Gravity over time
        velocity.y += gravity * Time.deltaTime;

        // 5. THE ONLY MOVE CALL
        // Combine horizontal (move * moveSpeed) and vertical (velocity)
        Vector3 finalFrameMovement = (move * moveSpeed) + velocity;
        controller.Move(finalFrameMovement * Time.deltaTime);
    }
    private void RotationLogic()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        xRotation  -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }



    public void OnMove(InputValue value) 
    
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
    public void OnJump(InputValue value)
    {
        jumpPressed = value.isPressed;
    }

}
