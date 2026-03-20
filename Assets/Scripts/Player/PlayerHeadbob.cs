using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]

//Controls a simple head bob camera effect to add realism and potentially help with motion sickness
public class PlayerHeadbob : MonoBehaviour
{

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float bobSpeed = 14f;
    [SerializeField] private float bobAmount = 0.02f;

    private PlayerMovement movement;
    private Vector3 originalCamPos;
    private float timer;

    private void Awake()
    {
        //Link to player movement as we use that for current speed and grounded
        movement = GetComponent<PlayerMovement>();
        originalCamPos = cameraTransform.localPosition;
    }

    private void Update()
    {
        float speed = movement.CurrentSpeed;
        bool isGrounded = movement.IsGrounded;
        
        if (isGrounded && speed > 0.2f)
        {
            //bob
            timer += Time.deltaTime * speed * bobSpeed;
            float bobY = Mathf.Sin(timer) * bobAmount;
            float bobX = Mathf.Cos(timer * 0.5f) * bobAmount;
            cameraTransform.localPosition = originalCamPos + new Vector3(bobX, bobY, 0f);
        }
        else
        {
            // not bobbing, rest state
            cameraTransform.localPosition =
                Vector3.Lerp(cameraTransform.localPosition, originalCamPos, Time.deltaTime * 8f);
        }
    }
}
