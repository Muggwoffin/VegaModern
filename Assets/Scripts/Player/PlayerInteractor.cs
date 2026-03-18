using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Image reticleImage;
    [SerializeField] private Color normalReticleColor = Color.beige;
    [SerializeField] private Color interactReticleColor = Color.green;

    public void Update()
    {
        UpdateReticle();
    }

    public void TryInteract()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
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
                if (hit.collider.TryGetComponent(out IInteractable _))
                {
                    reticleImage.color = interactReticleColor;
                    return;
                }
            }
        reticleImage.color = normalReticleColor;
    }
}
