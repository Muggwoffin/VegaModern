using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovement))]

//Handles first person camera look and the body rotation
//Uses input from the camera to rotate the camera up down and the body left and right
// Adds an FOV slider - again to tackle motion sickness - that zooms in and out depending on the current speed, which we have calculated in player movement.
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 20f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float zoomFOV = 65f;
    [SerializeField] private float fovLerpSpeed = 5f;
    [SerializeField] private float maxFOVSpeed = 5f;
    
    private PlayerInputHandler input;
    private PlayerMovement movement;
    private float xRotation = 0f; // Tracks verticle camera rotation

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        movement = GetComponent<PlayerMovement>();

        //Start with a neutral camera rotation
        xRotation = 0f;
        cameraTransform.localRotation = Quaternion.identity;
        
        //Lock the cursor to the centre
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Split responsibilities for clarity
        HandleLook();
        HandleFOV();
    }

    private void HandleLook()
    {
        Vector2 lookInput = input.LookInput;
        {
            //Scale by an adjustable sensitivity slider
            float mouseX = lookInput.x * mouseSensitivity;  
            float mouseY = lookInput.y * mouseSensitivity;  
    
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); //Add in the -90 and 90 prevents from flipping over
            
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    private void HandleFOV()
    {
        //Previously the FOV was more of a sprint zoom, now it is more of a soft speed-based effect. That's what this math handles
        float t = Mathf.Clamp01(movement.CurrentSpeed / maxFOVSpeed);
        float targetFOV = Mathf.Lerp(normalFOV, zoomFOV, t);
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);

    }
}
