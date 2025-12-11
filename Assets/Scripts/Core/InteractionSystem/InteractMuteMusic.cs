using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractMuteMusic : MonoBehaviour, IInteractable
{
    private bool isPaused = false;
    [SerializeField] private Animator animator;
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            AudioManager.instance.PauseMusic();
            animator.enabled = false;
        }
        else
        {
            animator.enabled = true;
            AudioManager.instance.UnpauseMusic();
        } 
    }
}
