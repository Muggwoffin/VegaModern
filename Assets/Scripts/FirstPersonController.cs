using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;
    public float moveSpeed = 3f;
    public float mouseSensitivity =20f;
    private Vector3 velocity;
    public float gravity = -9.8f;
    private Vector2 moveInput;
    private Vector2 lookInput;
    public float jumpHeight = 1.0f;
    private bool jumpPressed;

    [Header("Headbob")] public float bobSpeed = 14f;
    public float bobAmount = 0.02f;
    public float swayAmount = 0.1f;
    private Vector3 originalCamPos;
    private float timer = 0f;
    
    private float xRotation = 0f;
    private PlayerInput playerInput;

    private void Awake()
    {
        originalCamPos = cameraTransform.localPosition;
        {
            // 0 = Don't sync, 1 = Sync to monitor (V-Sync)
            QualitySettings.vSyncCount = 1; 
            
        }
        
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {

        MoveCharacter();
        RotationLogic();
        HeadBob();

    }
    
    private void HeadBob()
    {
        float currentSpeed = GetMovementSpeed();
        if (controller.isGrounded && currentSpeed > 0.2f)
        {
            timer += Time.deltaTime * currentSpeed * bobSpeed;
            float bobY = Mathf.Sin(timer) *  bobAmount;
            float bobX = Mathf.Cos(timer * 0.5f) *  bobAmount;
            cameraTransform.localPosition = originalCamPos + new Vector3(bobX, bobY, 0f);
        }
        else
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalCamPos, Time.deltaTime * 8f);
        }
    }
    private void MoveCharacter()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }


            if (jumpPressed)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpPressed = false; 
            }
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalFrameMovement = (move * moveSpeed) + velocity;
        controller.Move(finalFrameMovement * Time.deltaTime);
    }
    private void RotationLogic()
    {
        float mouseX = lookInput.x * mouseSensitivity;  
        float mouseY = lookInput.y * mouseSensitivity;  
    
        xRotation -= mouseY;
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

    private float GetMovementSpeed()
    {
        return new Vector3(moveInput.x, 0, moveInput.y).magnitude * moveSpeed;
    }

}
