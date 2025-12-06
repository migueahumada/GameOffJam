using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractMuteMusic : MonoBehaviour, IInteractable
{
    private bool isPaused = false;
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        isPaused = !isPaused;
        if (isPaused) AudioManager.instance.PauseMusic();
        else AudioManager.instance.UnpauseMusic();
    }
}
