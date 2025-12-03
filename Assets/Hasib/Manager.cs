using UnityEngine;

public class Manager : MonoBehaviour
{
    bool isPaused = false;
    [SerializeField] GameObject pauseMenu;

    public GameObject dialogueObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1;
        dialogueObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                HidePauseMenu();
            }
            else
            {
                ShowPauseMenu();
            }
           
        }
    }

    public void ShowPauseMenu()
    {
        if (AudioManager.instance != null)
        {
            // Calls lerp function to change the pause parameter in fmod
            AudioManager.instance.PauseMusicTransition();
        }

        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        isPaused = !isPaused;
    }

    public void HidePauseMenu()
    {
        if (AudioManager.instance != null)
        {
            // Calls lerp function to change the pause parameter in fmod
            AudioManager.instance.UnpauseMusicTransition();
        }
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        isPaused = !isPaused;
    }
    
}
