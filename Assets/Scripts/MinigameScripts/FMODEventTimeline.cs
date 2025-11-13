using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using System;

[System.Serializable]
public class TimelineEvent
{
    public float time_ms;
    public string eventName;
}

public class FMODEventTimeline : MonoBehaviour
{
    [Header("FMOD Event")]
    public EventReference fmodMusic;

    [Header("JSON de eventos (arrastrar desde Inspector)")]
    public TextAsset jsonFile;

    [Header("Configuración")]
    public bool autoStart = true;

    private EventInstance musicInstance;
    private List<TimelineEvent> timelineEvents = new List<TimelineEvent>();
    private int nextEventIndex = 0;

    public float CurrentTimeSeconds { get; private set; }
    public List<TimelineEvent> Events => timelineEvents;

    // ✅ Add this
    public List<RhythmAction> pairedActions { get; private set; }

    // ✅ Global event to notify others
    public static event Action<TimelineEvent> OnTimelineEventTriggered;

    void Start()
    {
        if (jsonFile == null) return;

        // --- Parse JSON ---
        string wrappedJson = "{\"events\":" + jsonFile.text + "}";
        TimelineEventWrapper wrapper = JsonUtility.FromJson<TimelineEventWrapper>(wrappedJson);
        timelineEvents = wrapper.events ?? new List<TimelineEvent>();
        timelineEvents.Sort((a, b) => a.time_ms.CompareTo(b.time_ms));

        // --- Pair start/finish events ---
        pairedActions = RhythmAction.PairEvents(timelineEvents);
        Debug.Log($"Paired {pairedActions.Count} actions from {timelineEvents.Count} events.");

        // --- FMOD setup ---
        musicInstance = RuntimeManager.CreateInstance(fmodMusic);
        if (autoStart) musicInstance.start();
    }

    void Update()
    {
        if (timelineEvents.Count == 0 || nextEventIndex >= timelineEvents.Count)
            return;

        musicInstance.getTimelinePosition(out int currentPosition);
        CurrentTimeSeconds = currentPosition / 1000f;

        while (nextEventIndex < timelineEvents.Count &&
               currentPosition >= timelineEvents[nextEventIndex].time_ms)
        {
            OnTimelineEventTriggered?.Invoke(timelineEvents[nextEventIndex]);
            nextEventIndex++;
        }
    }

    void OnDestroy()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }
    }

    [System.Serializable]
    private class TimelineEventWrapper
    {
        public List<TimelineEvent> events;
    }
}
