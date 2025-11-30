using System;
using UnityEngine;
using UnityEngine.UI;

public class SetttingsManager : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;
    [SerializeField] private Slider Music;
    [SerializeField] private Slider SFX;
    [SerializeField] private float defaultValue = 0.75f;

    [SerializeField]
    float currentSFXValue;
    [SerializeField]
    float currentMusicValue;
    private void Awake()
    {
        Music.value = defaultValue;
        SFX.value = defaultValue;
    }


   

    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
    }
    
    public void HideSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void SetSFXValue()
    {
        currentSFXValue = SFX.value;
        Debug.Log("CSF: "+currentSFXValue);
    }

    public void SetMusicValue()
    {
        currentMusicValue =Music.value;
        Debug.Log("CM: "+currentMusicValue);
    }
}
