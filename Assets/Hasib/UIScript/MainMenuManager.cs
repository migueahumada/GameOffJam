using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            
        }
    }
    public void LoadGame(int level)
    {
        SceneManager.LoadScene(level);

    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    public void ErasePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("playerprefs deleted");
    }
}
