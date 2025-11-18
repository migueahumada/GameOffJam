using UnityEngine;

public class IFillUpMachine : MonoBehaviour, IInterectable
{
    [SerializeField] private GameObject textPrompt;
    bool isCompleted = false; 
    public void Interect()
    {
        if (!isCompleted)
        {
            Debug.Log("Interected with fillup");
            isCompleted = !isCompleted;
        }
    }

    public void ShowInterectionText()
    {
        if (!isCompleted)
        {
            Debug.Log("Press E");
            textPrompt.SetActive(true);
        }
    }

    public void HideInterectionText()
    {
        textPrompt.SetActive(false);
    }
}
