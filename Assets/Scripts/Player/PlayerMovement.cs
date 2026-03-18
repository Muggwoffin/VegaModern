using System;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerMovement : MonoBehaviour
{

   
    public float moveSpeed = 3f;
    public float gravity = -9.8f;
    public float briskMultiplier = 1.5f;
    public float jumpHeight = 1.0f;
    public float acceleration = 10f;
    public float decceleration = 12f;

    public float CurrentSpeed
    {
        get
        {
            float baseSpeed = input.IsBrisk ? moveSpeed * briskMultiplier : moveSpeed;
            return input.MoveInput.magnitude * baseSpeed;
        }
    }
    
    private CharacterController controller;
    private PlayerInputHandler input;
    private Vector3 velocity;
    private Vector3 currentMoveVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        MoveCharacter();
    }
    private void MoveCharacter()
    {
        Vector2 moveInput = input.MoveInput;
        
        Vector3 move = Vector3.right * moveInput.x + Vector3.forward * moveInput.y;
        
        float currentSpeed = input.IsBrisk ? moveSpeed * briskMultiplier : moveSpeed;
        
        Vector3 desiredVelocity = move * currentSpeed;
        

        if (move.magnitude > 0.01f)
        {
            currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, desiredVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            currentMoveVelocity = Vector3.MoveTowards(currentMoveVelocity, Vector3.zero, decceleration * Time.deltaTime);
        }

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }


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
