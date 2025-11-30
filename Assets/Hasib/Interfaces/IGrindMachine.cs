using UnityEngine;

public class IGrindMachine : MonoBehaviour,IInterectable
{
    [SerializeField] private GameObject showGamePanel;
    bool isCompleted = false; 
    public void Interect()
    {
        if (!isCompleted)
        {
            Debug.Log("Load Grind Game");
            
            isCompleted = !isCompleted;
        }
    }

    public void ShowInterectionText()
    {
        if (!isCompleted)
        {
            showGamePanel.SetActive(true);
        }
    }

    public void HideInterectionText()
    {
        showGamePanel.SetActive(false);
    }
}
