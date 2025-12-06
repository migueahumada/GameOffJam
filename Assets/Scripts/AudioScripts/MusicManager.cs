using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.SceneManagement;
using FMODUnityResonance;

public class AudioManager : MonoBehaviour
{
    // --- Variables de Control ---
    private float PauseVal = 0.0f; // 0.0f = Reproduciendo, 1.0f = Pausado
    private float PAUSE_TIME = 0.7f; // Duración de la transición (fade)
    private Coroutine currentTransition; // Referencia para evitar conflictos de corrutinas
    public static AudioManager instance;
    private FMOD.Studio.EventInstance currentMusicInstance;

    // --- Estructura de Música por Escena ---
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
    
    // =======================================================
    // CICLO DE VIDA
    // =======================================================

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (currentMusicInstance.isValid())
        {
            currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentMusicInstance.release();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }
    
    // =======================================================
    // MÚSICA DE ESCENA
    // =======================================================

    /// <summary>
    /// Detiene la música actual, resetea el estado de pausa e inicia la nueva canción.
    /// </summary>
    public void PlayMusicForScene(string sceneName)
    {
        // Detener la música anterior
        if (currentMusicInstance.isValid())
        {
            currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentMusicInstance.release();
        }

        // Buscar el evento de música
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

        // Reproducir la nueva música y asegurar el estado inicial
        if (foundMusic && !newMusicEvent.IsNull)
        {
            currentMusicInstance = RuntimeManager.CreateInstance(newMusicEvent);
            // Aseguramos que la música empiece en estado "no pausado"
            PauseVal = 0f; 
            currentMusicInstance.setParameterByName("Paused", PauseVal);
            currentMusicInstance.start();
            //Debug.Log($"🎶 Reproduciendo música para la escena: **{sceneName}**");
        }
        else
        {
            Debug.Log($"🛑 No se encontró música o el evento es nulo para la escena: **{sceneName}**. La música se detuvo.");
        }
    }

    // =======================================================
    // CONTROL DE PAUSA FLUIDA
    // =======================================================

    /// <summary>
    /// Inicia la transición fluida del parámetro "Paused" a 1 (Pausado).
    /// </summary>
    public void PauseMusicTransition()
    {
        // 1f = destino (Pausado). PAUSE_TIME = duración.
        StartTransition(1f, PAUSE_TIME);
        Debug.Log("Iniciando transición a estado de pausa.");
    }

    /// <summary>
    /// Inicia la transición fluida del parámetro "Paused" a 0 (No Pausado).
    /// </summary>
    public void UnpauseMusicTransition()
    {
        // 0f = destino (Reproduciendo). PAUSE_TIME = duración.
        StartTransition(0f, PAUSE_TIME);
        Debug.Log("Iniciando transición a estado de reproducción.");
    }

    /// <summary>
    /// Método de utilidad para detener la transición anterior e iniciar una nueva.
    /// </summary>
    /// <param name="targetValue">El valor final (0f o 1f).</param>
    /// <param name="duration">Duración de la transición.</param>
    private void StartTransition(float targetValue, float duration)
    {
        // 1. Detener la corrutina anterior para evitar conflictos.
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        // 2. Iniciar la nueva transición, usando el valor actual de PauseVal como inicio.
        currentTransition = StartCoroutine(
            ChangeSmoothParameter(PauseVal, targetValue, duration)
        );
    }

    public void PauseMusic()
    {
        currentMusicInstance.setPaused(true);
    }
    public void UnpauseMusic()
    {
        currentMusicInstance.setPaused(false);
    }

    /// <summary>
    /// Corrutina para interpolar el valor del parámetro y aplicarlo a la instancia de música,
    /// utilizando tiempo no escalado para que funcione en pausa (Time.timeScale = 0).
    /// </summary>
    IEnumerator ChangeSmoothParameter( float v_start, float v_end, float duration )
    {
        float elapsed = 0.0f;
        
        if (!currentMusicInstance.isValid())
        {
            currentTransition = null; // Limpiar si la instancia no es válida
            yield break;
        }

        while (elapsed < duration )
        {
            // 1. Interpolación (suavizado)
            // PauseVal se actualiza entre v_start y v_end.
            PauseVal = Mathf.Lerp( v_start, v_end, elapsed / duration );
            
            // 2. Aplicar el valor fluido a FMOD
            currentMusicInstance.setParameterByName("Paused", PauseVal);
            
            // 3. Incrementar el tiempo usando Time.unscaledDeltaTime (permite ejecución en Time.timeScale = 0)
            elapsed += Time.unscaledDeltaTime;     
            
            yield return null;
        }

        // 4. Asegurar el valor final y limpiar la referencia
        PauseVal = v_end;
        currentMusicInstance.setParameterByName("Paused", PauseVal);
        currentTransition = null;
    }
}