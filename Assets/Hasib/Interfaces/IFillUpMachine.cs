using System;
using UnityEngine;

public class IFillUpMachine : MonoBehaviour, IInterectable
{
    [SerializeField] private GameObject showGamePanel;
    bool isCompleted = false; 
    public void Interect()
    {
        if (!isCompleted)
        {
            Debug.Log("Load fillup game");
            MainMenuManager.instance.LoadGame(2);
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
