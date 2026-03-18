using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public void Awake()
    {
        interactor = GetComponent<PlayerInteractor>();
    }

    public Vector2 MoveInput {get; private set;}
    public Vector2 LookInput {get; private set;}
    public bool IsBrisk {get; private set;}
    public bool JumpPressed {get; private set;}
    
    private PlayerInteractor interactor;
    
    public void OnMove(InputValue value) 
    
    {
        MoveInput = value.Get<Vector2>();
    }
    public void OnLook(InputValue value)
    {
        LookInput = value.Get<Vector2>();
    }

    public void OnBrisk(InputValue value)
    {
        if (value.isPressed)
        {
            IsBrisk = !IsBrisk;
        }
    }
    public void OnJump(InputValue value)
    {
        JumpPressed = value.isPressed;
    }

    public void EndJump()
    {
        JumpPressed = false;
    }
    
    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;
        interactor?.TryInteract();
    }
}
