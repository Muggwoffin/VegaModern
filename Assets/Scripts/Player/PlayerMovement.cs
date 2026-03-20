using System;
using UnityEngine;
using UnityEngine.InputSystem;

//Script for first-person walking, jumping and sprinting using a Character controller
// The movement is relative to where the camera is facing. Smoothed acceleration and deceleration helps to combat motion sickness
// It is connected to the Character Controller and the Player Input HAndler (which handles the keyboard/controller inputs)
// Horizontal movement is tied to input calculations whereas vertical movement (jumping) is physics based.

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{

   [SerializeField] private Transform cameraTransform; // This transform allows us to move relative to where the camera is looking
   
   [Header("Movement tuning")]
    public float moveSpeed = 3f;
    public float briskMultiplier = 1.5f;
    public float acceleration = 10f;
    public float decceleration = 12f;
    
    [Header ("Jump tuning")]
    public float gravity = -9.8f;
    public float jumpHeight = 1.0f;
    
    // Exposes a current speed value to other scripts, like the headbob, so that it is all synced up nicely.
    public float CurrentSpeed
    {
        get
        {
            float baseSpeed = input.IsBrisk ? moveSpeed * briskMultiplier : moveSpeed;
            return input.MoveInput.magnitude * baseSpeed;
        }
    }

    //Character controller has an in-built grounded check. This creates a pass trhrough
    public bool IsGrounded
    {
        get { return controller.isGrounded; }
    }
    
    private CharacterController controller;
    private PlayerInputHandler input;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity; //stores smoothed horizontal movement

    private void Awake()
    {
        //Cache the required components to avoid repeated movement calls.
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        //All per frame movement kept to one place for clarity
        MoveCharacter();
    }
    private void MoveCharacter()
    {
        // read 2D movement (x = strafe, y = forward)
        Vector2 moveInput = input.MoveInput;
        // get the camera based directions so movement is tied to where the player is looking
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        
        //avoid vertical tilt to keep us on the ground plane
        camForward.y = 0f;
        camRight.y = 0f;
        
        camForward.Normalize();
        camRight.Normalize();
        
        // Convert 2D input into 3D directions
        Vector3 move = camRight * moveInput.x + camForward * moveInput.y;
        
        //Choose walking or brisk movement depending on input (here's where we are feeding back into the player input component once more)
        float currentSpeed = input.IsBrisk ? moveSpeed * briskMultiplier : moveSpeed;
        
        // Target horizontal velocity for this frame before smoothing
        Vector3 desiredVelocity = move * currentSpeed;
        
        // I am liable to motion sickness in games but deceleration/acceleration helps. This script allows for hat using velocity
        if (move.magnitude > 0.01f)
        {
            currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, desiredVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, Vector3.zero, decceleration * Time.deltaTime);
        }
        
        // Using a grounded check and velocity on the y axis to control jumping
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }

        // Sqrt is a kinematic equation useful for calculating jump height.
            if (input.JumpPressed)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                input.EndJump();
            }
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalFrameMovement = currentMoveVelocity + velocity;
        controller.Move(finalFrameMovement * Time.deltaTime);
    }
}
