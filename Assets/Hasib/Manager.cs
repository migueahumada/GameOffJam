using UnityEngine;

public class Manager : MonoBehaviour
{
    bool isPaused = false;
    [SerializeField] GameObject pauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        isPaused = !isPaused;
    }

    public void HidePauseMenu()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        isPaused = !isPaused;
    }
}
