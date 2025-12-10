using System;
using System.Collections;
using FMODUnity;
using MinigameScripts;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private GameObject timelineObject; 
    [SerializeField] private GameObject rythmJudge;
    [SerializeField] private GameObject pauseMenu;
    [Header("FMOD Events")] 
    public EventReference musicEvent;
    public EventReference tutorialEvent;
    [Header("JSON files")] 
    public TextAsset musicJSON;
    public TextAsset tutorialJSON;

    [Header("Background on GameOver")] 
    [SerializeField] private GameObject _backgroundWin;
    [SerializeField] private GameObject _backgroundLoose;

    [Header("Caratulas")] 
    [SerializeField] private GameObject _caratula;
    [SerializeField] private EventReference _caratulaMusical;

    [Header("Skip Button")] 
    [SerializeField] private GameObject skipButton;

    [Header("Dialogues")]
    [SerializeField] private GameObject[] _dialogues;
    [SerializeField] private GameObject[] _tutoCompletedDialogues;

    [SerializeField] private GameObject settingsObject;
    public bool isUpEnabled = false;
    private bool isPaused = false;

    private GameObject[] currentDialogue;
    private int dialogIndex;
    private bool isIntroductionActive = false;

    private int stageIndex;
    private bool victory = false;

    private GameObject minigameObject;
    private GameObject tutorialObject;
    [SerializeField] private string destroyTag = null;

    public static event Action OnPause;
    public static event Action OnInputDown;
    public static event Action OnInputUp;
    public static event Action OnSkip;


    //private bool passTutorial = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stageIndex = 0;
        NextStage(false);
    }

    void OnEnable()
    {
        RhythmJudge.OnGameOverWin += HandleOnGameOverWin;
        RhythmJudge.OnGameOverLoose += HandleOnGameOverLoose;
        MenuManager.OnContinue += HandleOnContinue;
    }

    void OnDisable()
    {
        RhythmJudge.OnGameOverWin -= HandleOnGameOverWin;
        RhythmJudge.OnGameOverLoose -= HandleOnGameOverLoose;
        MenuManager.OnContinue -= HandleOnContinue;
    }

    private void HandleOnContinue()
    {
        skipButton.SetActive(isPaused);
        OnPause?.Invoke();
        isPaused = !isPaused;
        ShowMenu();
    }

    private void HandleOnGameOverWin()
    {
        Debug.Log("Game over win desde minigame manager");
        victory = true;
        NextStage(true);
    }    
    private void HandleOnGameOverLoose()
    {
        Debug.Log("Game over loose desde minigame manager");
        victory = false;
        NextStage(true);
    }

    public void NextStage(bool proceed)
    {
        if (proceed) stageIndex += 1;
        Debug.Log(stageIndex);
        switch (stageIndex)
        {
            case 0:
                ShowCaratula();
                ShowDialogue(_dialogues);
                break;
            case 1:
                DestroyLevel();
                StartTutorial();
                break;
            case 2:
                ShowDialogue(_tutoCompletedDialogues);
                break;
            case 3: 
                Invoke("DestroyLevel",1);
                Invoke("StartMinigame", 2);
                break;
            case 4:
                Invoke("DestroyLevel",2);
                Invoke("ShowGameOverScene",2);
                break;
            case 5:
                Invoke("ToTheHub",4);
                break;
        }
    }

    private void ShowCaratula()
    {
        _caratula.SetActive(true);
        _caratula.GetComponent<Animator>().SetTrigger("Start");
        RuntimeManager.PlayOneShot(_caratulaMusical);
    }

    private void ShowGameOverScene()
    {
        Debug.Log("SHOW GAME OVER");
        if (victory) _backgroundWin.SetActive(true);
        else _backgroundWin.SetActive(true);
        NextStage(true);
    }

    private void ToTheHub()
    {
        SceneManager.LoadScene(1);
    }

    private void ShowDialogue(GameObject[] dialogue)
    {
        currentDialogue = dialogue;
        Debug.Log("SHOW INTRODUCTION");
        //show the dialogues
        isIntroductionActive = true;
        dialogIndex = 0;
        if (currentDialogue.Length == 0) NextStage(true);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentDialogue != null && currentDialogue.Length > 0)
        {
            currentDialogue[dialogIndex].SetActive(true);
        }
    }

    private void AdvanceDialogue()
    {
        SFXManager.instance.PlaySFX(2);
        dialogIndex++;
        currentDialogue[dialogIndex-1].SetActive(false);
        if (dialogIndex < currentDialogue.Length) UpdateUI();
        else
        {
            isIntroductionActive = false;
            NextStage(true);
        }
    }

    public void SkipButton()
    {

        OnSkip?.Invoke();
        skipButton.SetActive(false);
        stageIndex = 3;
        NextStage(false);
    }

    private void DestroyLevel()
    {
        if (!string.IsNullOrEmpty(destroyTag))
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag(destroyTag).Length; ++i)
            {
                Destroy(GameObject.FindGameObjectsWithTag(destroyTag)[i]);
            }
        }
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Timeline").Length; ++i)
        {
            Destroy(GameObject.FindGameObjectsWithTag("Timeline")[i]);
        }
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("RythmJudge").Length; ++i)
        {
            Destroy(GameObject.FindGameObjectsWithTag("RythmJudge")[i]);
        }
    }
    

    private void StartTutorial()
    {
        Debug.Log("START TUTORIAL");
        skipButton.SetActive(true);
        //set and instanciate the tutorial object
        tutorialObject = Instantiate(timelineObject);

        FMODEventTimeline timelineScript = tutorialObject.GetComponent<FMODEventTimeline>();

        timelineScript.jsonFile = tutorialJSON;
        timelineScript.fmodMusic = tutorialEvent;

        timelineScript.InitializeTimeline();
        Debug.Log($"Tutorial music is: {tutorialObject.GetComponent<FMODEventTimeline>().fmodMusic}");
        //tutorialObject.GetComponent<FMODEventTimeline>().jsonFile = tutorialJSON;
        GameObject judgeInstanceTuto = Instantiate(rythmJudge);
        judgeInstanceTuto.GetComponent<RhythmJudge>().inputUpEnabled = isUpEnabled;
        judgeInstanceTuto.GetComponent<RhythmJudge>()._timeline = tutorialObject.GetComponent<FMODEventTimeline>();

        // iniciar tutorial con settings de cancion
    }
    private void StartMinigame()
    {
        Debug.Log("START MINIGAME");
        skipButton.SetActive(false);   
        //set and instanciate the minigame object
        minigameObject = Instantiate(timelineObject);

        FMODEventTimeline timelineScript = minigameObject.GetComponent<FMODEventTimeline>();

        timelineScript.jsonFile = musicJSON;
        timelineScript.fmodMusic = musicEvent;
        timelineScript.InitializeTimeline();

        Debug.Log($"Minigame music is: {minigameObject.GetComponent<FMODEventTimeline>().fmodMusic}");
        minigameObject.GetComponent<FMODEventTimeline>().jsonFile = musicJSON;
        // iniciar rythm judge con settings de cancion
        GameObject judgeInstance = Instantiate(rythmJudge);
        judgeInstance.GetComponent<RhythmJudge>().inputUpEnabled = isUpEnabled;
        judgeInstance.GetComponent<RhythmJudge>()._timeline = minigameObject.GetComponent<FMODEventTimeline>();
    }

    // Update is called once per frame
    void Update()
    {
        // INPUTS OF THE PLAYER 
        if (isIntroductionActive)
        {
            if (Input.GetKeyUp(KeyCode.Space) && dialogIndex < currentDialogue.Length)
            {
                AdvanceDialogue();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                //cambiar logica por eventos
                OnInputDown?.Invoke();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                OnInputUp?.Invoke();
            }    
        }
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (settingsObject.activeSelf)
            {
                settingsObject.SetActive(false);
            }
            else
            {
                if (stageIndex<3) skipButton.SetActive(isPaused);
                OnPause?.Invoke();
                isPaused = !isPaused;
                ShowMenu();    
            }
        }
    }

    private void ShowMenu()
    {
        pauseMenu.SetActive(isPaused);
    }
}
