using System;
using UnityEngine;
using UnityEngine.InputSystem;

//Keeps in one location all the input from the Unity Input system
// Boils down moving, looking, sprinting and jumping into simple properties
// Other scripts (the movement, look, and interactor script) read these each frame instead of talking directly to the Input system.
public class PlayerInputHandler : MonoBehaviour
{
    public void Awake()
    {
        //Cache the PlayerInteractor so we can enable interactions from input
        interactor = GetComponent<PlayerInteractor>();
    }

    //Read only properties that other scripts can query - Vector2s (because move uses WASD and Look uses mouse) and bools because jump and sprint are true false.
    public Vector2 MoveInput {get; private set;}
    public Vector2 LookInput {get; private set;}
    public bool IsBrisk {get; private set;}
    public bool JumpPressed {get; private set;}
    
    //Handles ray cast based interactions
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
        // Only trigger on the press, not on release
        if (!value.isPressed) return;
        //Ask the player interactor to try interacting with whatever is in front of the player. Basically a shorthand version of an if statement
        interactor?.TryInteract();
    }
    
}
