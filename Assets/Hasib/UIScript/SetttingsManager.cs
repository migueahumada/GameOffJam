using System;
using UnityEngine;
using UnityEngine.UI;

public class SetttingsManager : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;
    [SerializeField] private Slider Music;
    [SerializeField] private Slider SFX;
    [SerializeField] private float defaultValue = 0.75f;
    private const string SFXSliderKey = "SfxSliderValue";
    private const string MusicSliderKey = "MusicSliderValue";
    [SerializeField]
    float currentSFXValue;
    [SerializeField]
    float currentMusicValue;
    private void Awake()
    {
        float SFXsavedValue = PlayerPrefs.GetFloat(SFXSliderKey, defaultValue);
        float MusicsavedValue = PlayerPrefs.GetFloat(MusicSliderKey, defaultValue);
        
        SFXManager.instance.sfxBus.setVolume(SFXsavedValue);
        SFXManager.instance.musicBus.setVolume(MusicsavedValue);

        Music.value = PlayerPrefs.GetFloat(MusicSliderKey, defaultValue);
        SFX.value = PlayerPrefs.GetFloat(SFXSliderKey, defaultValue);
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
        PlayerPrefs.SetFloat(SFXSliderKey, currentSFXValue);
        PlayerPrefs.Save();  
        SFXManager.instance.sfxBus.setVolume(currentSFXValue);
    }

    public void SetMusicValue()
    {
        currentMusicValue =Music.value;
        PlayerPrefs.SetFloat(MusicSliderKey, currentMusicValue);
        SFXManager.instance.musicBus.setVolume(currentMusicValue);
    }
}
