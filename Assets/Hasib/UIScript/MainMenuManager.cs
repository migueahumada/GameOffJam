using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadGame(int level)
    {
        SceneManager.LoadScene(level);

    }
    
}
