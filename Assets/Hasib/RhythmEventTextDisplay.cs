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
        timeline = FindObjectOfType<FMODEventTimeline>();
        if (textDisplay) textDisplay.text = "";
    }

    void HandleTimelineEvent(TimelineEvent evt)
    {
        if (timeline == null || textDisplay == null)
            return;

        string baseName = evt.eventName.Replace(" start", "").Replace(" finish", "");

        // START EVENT
        if (evt.eventName.EndsWith("start"))
        {
            var finish = timeline.Events.FirstOrDefault(e =>
                e.eventName == $"{baseName} finish" && e.time_ms > evt.time_ms);

            if (finish != null)
            {
                float duration = (finish.time_ms - evt.time_ms) / 1000f;
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

            // Clear text shortly after the finish
            CancelInvoke(nameof(ClearText));
            Invoke(nameof(ClearText), 0.2f); // clear after 0.2s (tweak as desired)
        }
    }

    void ClearText()
    {
        if (textDisplay)
            textDisplay.text = "";
    }
}