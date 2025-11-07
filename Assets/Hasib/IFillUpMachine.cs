using UnityEngine;

public class IFillUpMachine : MonoBehaviour, IInterectable
{
    
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
        }
    }
}
