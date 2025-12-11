using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // Closest Interactable
    [Header("ICONS")]
    [SerializeField] private GameObject minigameIcon;
    [SerializeField] private GameObject interactionIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionIcon.SetActive(false);
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactableInRange?.Interact();
            Debug.Log("Interacted!");
        }
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            if (other.gameObject.tag == "Minigame") minigameIcon.SetActive(true);
            else if (other.gameObject.tag == "Interactable") interactionIcon.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
            minigameIcon.SetActive(false);
        }   
    }
}
