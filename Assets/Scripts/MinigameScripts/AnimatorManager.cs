using System;
using MinigameScripts;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private string[] _tags;
    private Animator[] animatorArray;

    // variables

    private bool isPaused = false;

    [Header("Settings")]
    [SerializeField] private bool inputUpEnabled = true;



    void OnEnable()
    {
        // suscrito a todos los eventos que incluyen animaciones
        MinigameManager.OnPause += HandleOnPause;
        MinigameManager.OnInputDown += HandleOnInputDown;
        MinigameManager.OnInputUp += HandleOnInputUp;
        RhythmJudge.OnMiss += HandleOnMiss;
        //RhythmJudge.
    }

    private void HandleOnMiss()
    {
        for (int i = 0; i < animatorArray.Length; ++i)
        {
            animatorArray[i].SetBool("Pressed", false);
            animatorArray[i].SetTrigger("Miss");
        }
    }

    private void HandleOnInputUp()
    {
        if (inputUpEnabled)
        {
            for (int i = 0; i < animatorArray.Length; ++i)
            {
                animatorArray[i].SetBool("Pressed", false);
            } 
        }
    }

    private void HandleOnInputDown()
    {
        for (int i = 0; i < animatorArray.Length; ++i)
        {
            animatorArray[i].SetBool("Pressed", true);
        }
    }

    void OnDisable()
    {
        MinigameManager.OnPause -= HandleOnPause;
        MinigameManager.OnInputDown -= HandleOnInputDown;
        MinigameManager.OnInputUp -= HandleOnInputUp;
        RhythmJudge.OnMiss += HandleOnMiss;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void HandleOnPause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            for (int i = 0; i < animatorArray.Length; ++i)
            {    
                animatorArray[i].enabled = false;
            }
        } 
        else{
            for (int i = 0; i < animatorArray.Length; ++i)
            {    
                animatorArray[i].enabled = true;
            }
        }
    }
    void Start()
    {
        // En funcion de los tags que indique el usuario, encontrar los animators asociados.
        animatorArray = new Animator[_tags.Length];
        for (int i = 0; i < _tags.Length; ++i)
        {
            GameObject newAnimatedObject;
            newAnimatedObject = GameObject.FindGameObjectWithTag(_tags[i]);
            
            animatorArray[i] = newAnimatedObject.GetComponent<Animator>();
        }
    }


    // Llamar a todos los eventos para tener independecia de codigo entre el rythm judge y el animator y poder usarlo en todos lados

    // Update is called once per frame
    void Update()
    {
        
    }
}
