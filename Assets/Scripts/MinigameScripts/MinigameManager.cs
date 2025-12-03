using FMODUnity;
using MinigameScripts;
using UnityEngine;

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

    
    private GameObject minigameObject;
    private GameObject tutorialObject;

    //private bool passTutorial = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartMinigame();
       // StartTutorial();
    }


    private void StartTutorial()
    {
        //set and instanciate the tutorial object
        timelineObject.GetComponent<FMODEventTimeline>().jsonFile = tutorialJSON;
        timelineObject.GetComponent<FMODEventTimeline>().fmodMusic = tutorialEvent;
        tutorialObject = Instantiate(timelineObject);
        Debug.Log($"Tutorial music is: {tutorialObject.GetComponent<FMODEventTimeline>().fmodMusic}");
        //tutorialObject.GetComponent<FMODEventTimeline>().jsonFile = tutorialJSON;

        // iniciar tutorial con settings de cancion
        GameObject judgeInstanceTuto = Instantiate(rythmJudge);
        judgeInstanceTuto.GetComponent<RhythmJudge>()._timeline = tutorialObject.GetComponent<FMODEventTimeline>();
    }
    private void StartMinigame()
    {
        //set and instanciate the minigame object
        timelineObject.GetComponent<FMODEventTimeline>().jsonFile = musicJSON;
        timelineObject.GetComponent<FMODEventTimeline>().fmodMusic = musicEvent;
        minigameObject = Instantiate(timelineObject);
        
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
