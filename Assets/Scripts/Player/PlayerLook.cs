using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 20f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float zoomFOV = 65f;
    [SerializeField] private float fovLerpSpeed = 5f;
    
    private PlayerInputHandler input;
    private PlayerMovement movement;
    private float xRotation = 0f;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        movement = GetComponent<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleLook();
        HandleFOV();
    }

    private void HandleLook()
    {
        Vector2 lookInput = input.LookInput;
        {
            float mouseX = lookInput.x * mouseSensitivity;  
            float mouseY = lookInput.y * mouseSensitivity;  
    
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    private void HandleFOV()
    {
        float targetFOV = input.IsBrisk ? zoomFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
        
    }
}
