using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.SceneManagement; 

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // --- Música por Escena ---

    [System.Serializable]
    public class SceneMusicPair
    {
        public string sceneName;
        public EventReference musicEvent;
    }

    [Header("Scene Music Configuration")]
    [Tooltip("Define la música de fondo que se reproducirá para cada escena específica.")]
    [SerializeField]
    public List<SceneMusicPair> sceneMusicMap = new List<SceneMusicPair>();
    
    private FMOD.Studio.EventInstance currentMusicInstance;

    private void Awake()
    {
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
    }

    private void OnEnable()
    {
        // Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Cancelar suscripción al evento de cambio de escena
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // Detener la música al desactivarse el objeto
        if (currentMusicInstance.isValid())
        {
            currentMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentMusicInstance.release();
        }
    }

    // Método llamado cuando se carga una nueva escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    /// <summary>
    /// Detiene la música actual y reproduce la canción asignada a la nueva escena.
    /// </summary>
    /// <param name="sceneName">El nombre de la escena cargada.</param>
    public void PlayMusicForScene(string sceneName)
    {
        // 1. Detener y liberar la música actual si está reproduciéndose
        if (currentMusicInstance.isValid())
        {
            currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentMusicInstance.release();
        }

        // 2. Buscar el evento de música para la escena
        EventReference newMusicEvent = default;
        bool foundMusic = false;

        foreach (var pair in sceneMusicMap)
        {
            if (pair.sceneName == sceneName)
            {
                newMusicEvent = pair.musicEvent;
                foundMusic = true;
                break;
            }
        }

        // 3. Reproducir la nueva música si se encuentra
        if (foundMusic && !newMusicEvent.IsNull)
        {
            currentMusicInstance = RuntimeManager.CreateInstance(newMusicEvent);
            currentMusicInstance.start();
            Debug.Log($"🎶 Reproduciendo música para la escena: **{sceneName}**");
        }
        else
        {
            Debug.Log($"🛑 No se encontró música o el evento es nulo para la escena: **{sceneName}**. La música se detuvo.");
        }
    }
}