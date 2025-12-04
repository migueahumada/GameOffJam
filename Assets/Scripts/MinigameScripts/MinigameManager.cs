using System;
using FMODUnity;
using MinigameScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private GameObject timelineObject; 
    [SerializeField] private GameObject rythmJudge;
    [Header("FMOD Events")] 
    public EventReference musicEvent;
    public EventReference tutorialEvent;
    [Header("JSON files")] 
    public TextAsset musicJSON;
    public TextAsset tutorialJSON;

    [SerializeField] private GameObject _backgroundWin;
    [SerializeField] private GameObject _backgroundLoose;
    [SerializeField] private GameObject _caratula;
    [SerializeField] private EventReference _caratulaMusical;
    [SerializeField] private GameObject skipButton;

    private int stageIndex;
    private bool victory = false;

    private GameObject minigameObject;
    private GameObject tutorialObject;


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
    }

    void OnDisable()
    {
        RhythmJudge.OnGameOverWin -= HandleOnGameOverWin;
        RhythmJudge.OnGameOverLoose -= HandleOnGameOverLoose;
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
        NextStage(false);
    }

    public void NextStage(bool proceed)
    {
        if (proceed) stageIndex += 1;
        switch (stageIndex)
        {
            case 0:
                ShowIntroduction();
                break;
            case 1:
                Invoke("StartTutorial", 2);
                break;
            case 2: 
                Invoke("DestroyLevel",1);
                Invoke("StartMinigame", 0.5f);
                break;
            case 3:
                Invoke("DestroyLevel",2);
                Invoke("ShowGameOverScene",2);
                break;
            case 4:
                Invoke("ToTheHub",4);
                break;
        }
    }

    private void ShowGameOverScene()
    {
        if (victory) _backgroundWin.SetActive(true);
        else _backgroundWin.SetActive(true);
        NextStage(true);
    }

    private void ToTheHub()
    {
        SceneManager.LoadScene(1);
    }

    private void ShowIntroduction()
    {
        _caratula.SetActive(true);
        RuntimeManager.PlayOneShot(_caratulaMusical);
        Debug.Log("hola tienes que pulsar el teclado para ganar");
        NextStage(true);
    }

    private void DestroyLevel()
    {
        Destroy(GameObject.FindGameObjectWithTag("Timeline"));
        Destroy(GameObject.FindGameObjectWithTag("RythmJudge"));
    }

    private void StartTutorial()
    {
        _caratula.GetComponent<Animator>().SetTrigger("Start");
        //_caratula.SetActive(false);
        skipButton.SetActive(true);
        //set and instanciate the tutorial object
        tutorialObject = Instantiate(timelineObject);

        FMODEventTimeline timelineScript = tutorialObject.GetComponent<FMODEventTimeline>();

        timelineScript.jsonFile = tutorialJSON;
        timelineScript.fmodMusic = tutorialEvent;

        timelineScript.InitializeTimeline();
        Debug.Log($"Tutorial music is: {tutorialObject.GetComponent<FMODEventTimeline>().fmodMusic}");
        //tutorialObject.GetComponent<FMODEventTimeline>().jsonFile = tutorialJSON;

        // iniciar tutorial con settings de cancion
        GameObject judgeInstanceTuto = Instantiate(rythmJudge);
        judgeInstanceTuto.GetComponent<RhythmJudge>()._timeline = tutorialObject.GetComponent<FMODEventTimeline>();
    }
    private void StartMinigame()
    {
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
        judgeInstance.GetComponent<RhythmJudge>()._timeline = minigameObject.GetComponent<FMODEventTimeline>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
