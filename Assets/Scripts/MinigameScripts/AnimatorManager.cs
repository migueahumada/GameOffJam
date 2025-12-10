using System;
using FMODUnity;
using MinigameScripts;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [Header("ANIMATORS")]
    [SerializeField] private string[] _tags;
    private Animator[] animatorArray;

    // variables

    private bool isPaused = false;

    [Header("SETTINGS")]
    private bool inputUpEnabled;

    [Header("LEVEL")]
    [SerializeField] private string levelName;
    [SerializeField] private MinigameManager minigameManager;

    [Header("INPUT SOUNDS")]
    [SerializeField] private EventReference inputDown;
    [SerializeField] private EventReference inputUp;
    [SerializeField] private EventReference perfectRelease;
    [SerializeField] private EventReference perfectCatch;
    [SerializeField] private EventReference missInput;
    [SerializeField] private EventReference prepEvent;



    void OnEnable()
    {
        // suscrito a todos los eventos que incluyen animaciones
        MinigameManager.OnPause += HandleOnPause;
        MinigameManager.OnInputDown += HandleOnInputDown;
        MinigameManager.OnInputUp += HandleOnInputUp;
        RhythmJudge.OnMiss += HandleOnMiss;
        RhythmJudge.OnPerfectInput += HandleOnPerfectInput;
        FMODEventTimeline.OnTimelineEventTriggered += HandleTimelineEvent;
        RhythmJudge.OnPerfectRelease += HandleOnPerfectRelease;
        //RhythmJudge.
    }

    private void HandleOnPerfectRelease()
    {
        if (levelName == "FlavourFill")
        {
            RuntimeManager.PlayOneShot(perfectRelease);
            //SFXManager.instance.PlaySFX(6);
        }
    }

    private void HandleTimelineEvent(TimelineEvent evt)
    {
        String evtName = evt.eventName.ToLower();
        if (levelName == "GrindHour")
        {
            if (evtName.Contains("prep 4c start"))
            {
                for (int i = 0; i < animatorArray.Length; ++i)
                {
                    RuntimeManager.PlayOneShot(prepEvent);
                    animatorArray[i].SetTrigger("Prep");
                }
            }
        }
    }
    private void HandleOnMiss()
    {
        if (levelName == "FlavourFill")
        {
            RuntimeManager.PlayOneShot(missInput);
            for (int i = 0; i < animatorArray.Length; ++i)
            {
                animatorArray[i].SetBool("Pressed", false);
                animatorArray[i].SetTrigger("Miss");
            }
        }
        else if (levelName == "GrindHour")
        {
            RuntimeManager.PlayOneShot(missInput);
            for (int i = 0; i < animatorArray.Length; ++i)
            {
                animatorArray[i].SetTrigger("Miss");
            }
        }
    }

    private void HandleOnInputUp()
    {
        if (inputUpEnabled)
        {
            if (levelName == "FlavourFill")
            {
                //RuntimeManager.PlayOneShot(inputUp); MEJOR CONTROLAR DESDE ANIM PARA EVITAR SPAM
                for (int i = 0; i < animatorArray.Length; ++i)
                {
                    animatorArray[i].SetBool("Pressed", false);
                } 
            }   
        }
    }

    private void HandleOnInputDown()
    {
        if (levelName == "FlavourFill")
        {   
            //SFXManager.instance.PlaySFX(5);
            //RuntimeManager.PlayOneShot(inputUp);         
            for (int i = 0; i < animatorArray.Length; ++i)
            {
                animatorArray[i].SetBool("Pressed", true);
            }
        }
        else if (levelName == "GrindHour")
        {
            for (int i = 0; i < animatorArray.Length; ++i)
            {
                animatorArray[i].SetTrigger("Input");
            }
        }
    }

    private void HandleOnPerfectInput()
    {
        if (levelName == "GrindHour")
        {
            //SFXManager.instance.PlaySFX(5);
            RuntimeManager.PlayOneShot(perfectCatch);
            for (int i = 0; i < animatorArray.Length; ++i)
            {
                animatorArray[i].SetTrigger("Release");
            }
        }
    }

    void OnDisable()
    {
        MinigameManager.OnPause -= HandleOnPause;
        MinigameManager.OnInputDown -= HandleOnInputDown;
        MinigameManager.OnInputUp -= HandleOnInputUp;
        RhythmJudge.OnMiss -= HandleOnMiss;
        RhythmJudge.OnPerfectInput -= HandleOnPerfectInput;
        FMODEventTimeline.OnTimelineEventTriggered -= HandleTimelineEvent;
        RhythmJudge.OnPerfectRelease -= HandleOnPerfectRelease;
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
        inputUpEnabled = minigameManager.isUpEnabled;
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
