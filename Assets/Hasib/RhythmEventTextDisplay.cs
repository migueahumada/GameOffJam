using System;
using UnityEngine;
using TMPro;
using System.Linq;

public class RhythmEventTextDisplay : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI textDisplay;

    private FMODEventTimeline timeline;

    void OnEnable()
    {
        FMODEventTimeline.OnTimelineEventTriggered += HandleTimelineEvent;
    }

    void OnDisable()
    {
        FMODEventTimeline.OnTimelineEventTriggered -= HandleTimelineEvent;
    }

    void Start()
    {
        timeline = FindAnyObjectByType<FMODEventTimeline>();
        if (textDisplay) textDisplay.text = "";
    }

    void HandleTimelineEvent(TimelineEvent evt)
    {
        if (timeline == null || textDisplay == null)
            return;

        string baseName = evt.eventName.Replace(" start", "").Replace(" finish", "");
        
        // 1. Get the float time for the current event
        float evtTime = evt.GetTimeMs(); 

        // START EVENT
        if (evt.eventName.EndsWith("start"))
        {
            // 2. Find matching finish using GetTimeMs() for comparison
            TimelineEvent finish = timeline.Events.FirstOrDefault(e =>
                e.eventName == $"{baseName} finish" && e.GetTimeMs() > evtTime);

            if (finish != null)
            {
                // 3. Calculate duration using GetTimeMs()
                float duration = (finish.GetTimeMs() - evtTime) / 1000f;
                textDisplay.text = $"{baseName} (Duration: {duration:F3}s)";
            }
            else
            {
                textDisplay.text = $"{baseName} (no finish found)";
            }
        }
        // FINISH EVENT
        else if (evt.eventName.EndsWith("finish"))
        {
            textDisplay.text = $"{baseName} finished";

            CancelInvoke(nameof(ClearText));
            Invoke(nameof(ClearText), 0.2f); 
        }
    }

    void ClearText()
    {
        if (textDisplay)
            textDisplay.text = "";
    }
}