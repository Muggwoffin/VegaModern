using System;
using UnityEngine;
using UnityEngine.UI;

//Handles looking at and interacting with objects that are in front of the player.
// Shoots out a ray cast from the Camera Transform that checks if an object is linked to the IInteractable interface within a short range of the player
// Uses the UI system to change the reticle colour to provide a visual cue that the player can interact with this object
public class PlayerInteractor : MonoBehaviour
{

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRange = 3f;
    //Only hit objects that are within the interactable layer
    [SerializeField] private LayerMask interactableLayer;
    
    [Header("Reticle UI")]
    [SerializeField] private Image reticleImage;
    [SerializeField] private Color normalReticleColor = Color.beige;
    [SerializeField] private Color interactReticleColor = Color.green;

    public void Update()
    {
        UpdateReticle();
    }

    public void TryInteract()
    {
        // Builds a ray starting at the camera that looks forward
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        // Hit the ray forward and see if it hits something, hemmed in by our range and layers
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            //Check if the object is part of the interactable interface
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
                Debug.Log("Interacting with " + hit.collider.name);
            }
        }
    }

    public void UpdateReticle()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
            {
                //If the ray hits an object on the interactable range, the colour of the reticle changes green
                if (hit.collider.TryGetComponent(out IInteractable _))
                {
                    reticleImage.color = interactReticleColor;
                    return;
                }
            }
        //otherwise just keep the reticle white.
        reticleImage.color = normalReticleColor;
    }
}
