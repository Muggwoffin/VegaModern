
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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

    [Header("Brisk Walk")] 
    public float briskMultiplier = 1.5f;
    private bool isBrisk = false;
    
    [Header("UI")]
    public UnityEngine.UI.Image reticleImage;

    public Color normalReticleColor = Color.beige;
    public Color interactReticleColor = Color.darkGreen;
    
    [Header("Headbob")] 
    public float bobSpeed = 14f;
    public float bobAmount = 0.02f;
    public float swayAmount = 0.1f;
    private Vector3 originalCamPos;
    private float timer = 0f;

    [Header("Interact")] public float interactRange = 3f;
    public LayerMask interactableLayer;
    
    [Header("Gentle Movement")]
    public float acceleration = 10f;
    public float decceleration = 12f;
    Vector3 currentMoveVelocity;
    
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
        BriskWiden();
        MoveCharacter();
        RotationLogic();
        HeadBob();
        InteractableCheck();
    }

    private void InteractableCheck()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                reticleImage.color = interactReticleColor;
                return;
            }
        }
        reticleImage.color = normalReticleColor;
    }

    private void BriskWiden()
    {
        float targetFOV = isBrisk ? 65f : 60f;
        cameraTransform.GetComponent<Camera>().fieldOfView = Mathf.Lerp(
            cameraTransform.GetComponent<Camera>().fieldOfView, 
            targetFOV, 
            Time.deltaTime * 5f
        );
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
        
        float currentSpeed = isBrisk ? moveSpeed * briskMultiplier : moveSpeed;
        
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


            if (jumpPressed)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpPressed = false; 
            }
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalFrameMovement = currentMoveVelocity + velocity;
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

    public void OnBrisk(InputValue value)
    {
        if (value.isPressed)
        {
            isBrisk = !isBrisk;
        }
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            Debug.DrawRay(ray.origin, ray.direction + ray.direction * interactRange, Color.red, 2f);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, interactRange))
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact();
                    Debug.Log("Interacting with " + hit.collider.name);
                }
        }
    }
    public void OnJump(InputValue value)
    {
        jumpPressed = value.isPressed;
    }

    private float GetMovementSpeed()
    {
        float baseSpeed = isBrisk ? moveSpeed * briskMultiplier : moveSpeed;
        return moveInput.magnitude * baseSpeed;
    }

}
