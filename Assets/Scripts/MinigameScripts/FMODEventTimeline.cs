using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Globalization;
using MinigameScripts; // Needed for float parsing

[System.Serializable]
public class TimelineEvent
{
    // 1. Read the JSON string exactly as it is written
    public String time_ms; 
    public String eventName;

    // 2. Helper to convert string "5714.29" to float 5714.29f
    public Single GetTimeMs()
    {
        if (float.TryParse(time_ms, NumberStyles.Any, CultureInfo.InvariantCulture, out Single result))
        {
            return result;
        }
        return 0f;
    }
}

public class FMODEventTimeline : MonoBehaviour
{
    [Header("FMOD Event")]
    public EventReference fmodMusic;

    [Header("JSON Data")]
    public TextAsset jsonFile;

    [Header("Settings")]
    public Boolean autoStart = true;

    private bool pausedMenu = false;

    private EventInstance _musicInstance;
    private List<TimelineEvent> _timelineEvents = new();
    private Int32 _nextEventIndex;

    public Single CurrentTimeSeconds { get; private set; }
    
    // Public getter
    public List<TimelineEvent> Events => _timelineEvents ?? new List<TimelineEvent>();

    private RhythmJudge rhythmJudge;
    public static event Action<TimelineEvent> OnTimelineEventTriggered;

    private void OnEnable()
    {
        MinigameManager.OnPause += HandleOnPause;
        MinigameManager.OnSkip += HandleOnSkip;
    }
    private void OnDisable()
    {
        MinigameManager.OnPause -= HandleOnPause;
        MinigameManager.OnSkip -= HandleOnSkip;
    }

    private void HandleOnSkip()
    {
        _musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void InitializeTimeline()
    {
        // AHORA ESTO ES PUBLICO y se llama después de asignar 'jsonFile'

        if (jsonFile == null)
        {
            Debug.LogError("❌ FMODEventTimeline: No JSON file assigned!");
            return;
        }

        // Fix JSON array format manually
        string wrappedJson = "{\"events\":" + jsonFile.text + "}";

        try 
        {
            TimelineEventWrapper wrapper = JsonUtility.FromJson<TimelineEventWrapper>(wrappedJson);
            _timelineEvents = wrapper.events ?? new List<TimelineEvent>();

            // Sort by the converted float time
            _timelineEvents.Sort((a, b) => a.GetTimeMs().CompareTo(b.GetTimeMs()));

            if (_timelineEvents.Count > 0)
            {
                Debug.Log($"✅ Loaded {_timelineEvents.Count} events.");
                // ... logs
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ JSON Parse Error: {e.Message}");
        }
    }

    public void Start()
    {
        _musicInstance = RuntimeManager.CreateInstance(fmodMusic);
        
        if (autoStart) 
        {
            _musicInstance.start();
            Debug.Log("🎵 Music Started");
        }
    }

    public void HandleOnPause()
    {
        pausedMenu = !pausedMenu;
        _musicInstance.setPaused(pausedMenu);
        //if (pausedMenu) Time.timeScale = 0;
        //else Time.timeScale = 1;
    }

    private void Update()
    {
        if (!_musicInstance.isValid() || _timelineEvents.Count == 0) return;

        _musicInstance.getTimelinePosition(out Int32 currentPosition);
        CurrentTimeSeconds = currentPosition / 1000f;

        // Check against the converted float time
        while (_nextEventIndex < _timelineEvents.Count &&
               currentPosition >= _timelineEvents[_nextEventIndex].GetTimeMs())
        {
            OnTimelineEventTriggered?.Invoke(_timelineEvents[_nextEventIndex]);
            _nextEventIndex++;
        }
    }

    private void OnDestroy()
    {
        if (_musicInstance.isValid())
        {
            _musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _musicInstance.release();
        }
    }

    [Serializable]
    private class TimelineEventWrapper
    {
        public List<TimelineEvent> events;
    }
}