using System;
using Unity.VisualScripting;
using UnityEngine;

public class InterectionPoint : MonoBehaviour
{
    
    private IInterectable _currentInteractable;

    private void Update()
    {
        if (_currentInteractable!=null && Input.GetKeyDown(KeyCode.E))
        {
            _currentInteractable.Interect();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _currentInteractable = other.GetComponent<IInterectable>();
        if (_currentInteractable != null)
        {
            _currentInteractable.ShowInterectionText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        _currentInteractable = null;
    }
}
