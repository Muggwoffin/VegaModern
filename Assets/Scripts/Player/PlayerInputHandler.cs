using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput {get; private set;}
    public Vector2 LookInput {get; private set;}
    public bool IsBrisk {get; private set;}
    public bool JumpPressed {get; private set;}
    
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
        if (value.isPressed)
        {
            if (!value.isPressed) return;
        }
    }
}
