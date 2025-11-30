using UnityEngine;

public class SetttingsManager : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;
    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
    }
    
    public void HideSettings()
    {
        settingsPanel.SetActive(false);
    }
}
