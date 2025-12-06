using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractLoadLevel : MonoBehaviour, IInteractable
{
    [SerializeField] private string sceneName;
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        SceneManager.LoadScene(sceneName);
    }
}
