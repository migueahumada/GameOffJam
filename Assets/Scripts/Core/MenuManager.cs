using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject _settings;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    public static event Action OnContinue; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Continue()
    {
        Debug.Log("continue");
        OnContinue?.Invoke();
    }

    public void ShowSettings()
    {
        Debug.Log("showsettings");
        _settings.SetActive(true);
    }

    public void Menu()
    {
        Debug.Log("Menu");
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    public void MusicVolume()
    {
        SFXManager.instance.musicBus.setVolume(musicSlider.value);
    }
    public void SFXVolume()
    {
        SFXManager.instance.sfxBus.setVolume(sfxSlider.value);
    }
    public void HideSettings()
    {
        _settings.SetActive(false);
    }
}
