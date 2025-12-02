using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.SceneManagement;
using FMODUnityResonance;
using FMOD.Studio;

public class SFXManager : MonoBehaviour
{
    // Bus declaration
    public Bus musicBus;
    public Bus sfxBus;

    private Coroutine currentTransition; // Referencia para evitar conflictos de corrutinas
    public static SFXManager instance;
    FMODUnity.StudioEventEmitter eventEmitter;
    


    [Header("SFX Events Configuration")]
    [Tooltip("Define los efectos de sonido que hay.")]
    [SerializeField] EventReference[] SFX;

    
    // =======================================================
    // Position:
    // 0-Footsteps 1-ButtonHover 2-ButtonClick 3-Slider 4-AcceptDialogue
    // =======================================================

    private void Awake()
    {
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");

        // Implementación Singleton para que el AudioManager persista
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
        eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    public void PlaySFX(int sfxIndex)
    {
        // Verificamos que el índice sea válido para evitar errores
        if (sfxIndex >= 0 && sfxIndex < SFX.Length)
        {
            // PlayOneShot es ideal para UI porque no necesitas detenerlo después
            RuntimeManager.PlayOneShot(SFX[sfxIndex]);    
        }
        else
        {
            Debug.LogWarning($"SFXManager: Índice {sfxIndex} fuera de rango. Revisa el array SFX.");
        }
    }

    public void PlaySFXOneAtTime(int sfxIndex)
    {
        eventEmitter.EventReference = SFX[sfxIndex];
        if (!eventEmitter.IsPlaying())
        {
            eventEmitter.Play();
        }
    }

    public void PlaySFXAttached(int sfxIndex, GameObject gameObject)
    {
        // Verificamos que el índice sea válido para evitar errores
        if (sfxIndex >= 0 && sfxIndex < SFX.Length)
        {
            // PlayOneShot es ideal para UI porque no necesitas detenerlo después
            RuntimeManager.PlayOneShotAttached(SFX[sfxIndex], gameObject);    
        }
        else
        {
            Debug.LogWarning($"SFXManager: Índice {sfxIndex} fuera de rango. Revisa el array SFX.");
        }
    }

    public void PlaySFXReference(EventReference reference)
    {
        RuntimeManager.PlayOneShot(reference);
    }
}