using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractLoadLevel : MonoBehaviour, IInteractable
{
    [SerializeField] private int sceneIndex;
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
