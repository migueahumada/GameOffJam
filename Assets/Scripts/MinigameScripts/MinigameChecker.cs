using System;
using MinigameScripts;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameChecker : MonoBehaviour
{
    private bool isDone = false;
    private string sceneName;
    private Scene m_scene;
    private string minigameName;
    private int currentGames;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnEnable()
    {
        RhythmJudge.OnGameOverWin += HandleGameOverWin;
    }
    void OnDisable()
    {
        RhythmJudge.OnGameOverWin -= HandleGameOverWin;
    }

    private void HandleGameOverWin()
    {
        if (!isDone)
        {
            currentGames = PlayerPrefs.GetInt("gamesDone", 0);
            Debug.Log($"cuantos juegos se ha pasado el jugador: {currentGames}");
            PlayerPrefs.SetInt("gamesDone", currentGames+=1);
        }
    }

    void Start()
    {
        // comprobar y actualizar estado de minijuego
        m_scene = SceneManager.GetActiveScene();
        sceneName = m_scene.name;
        minigameName = PlayerPrefs.GetString(sceneName);
        if (String.IsNullOrEmpty(minigameName))
        {
            PlayerPrefs.SetString(sceneName, "notDone");
        }
        else if (PlayerPrefs.GetString(sceneName) == "done")
        {
            isDone = true;
        }
        else isDone = false;
    }
}
