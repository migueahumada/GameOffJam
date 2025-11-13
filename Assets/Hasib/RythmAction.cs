using System.Collections.Generic;

[System.Serializable]
public class RhythmAction
{
    public string name;
    public float startTimeMs;
    public float endTimeMs;
    public float DurationMs => endTimeMs - startTimeMs;

    public RhythmAction(string name, float start, float end)
    {
        this.name = name;
        this.startTimeMs = start;
        this.endTimeMs = end;
    }

    public static List<RhythmAction> PairEvents(List<TimelineEvent> events)
    {
        var pairs = new List<RhythmAction>();
        var stack = new Dictionary<string, float>();

        foreach (var e in events)
        {
            if (e.eventName.EndsWith("start"))
            {
                string baseName = e.eventName.Replace(" start", "");
                stack[baseName] = e.time_ms;
            }
            else if (e.eventName.EndsWith("finish"))
            {
                string baseName = e.eventName.Replace(" finish", "");
                if (stack.TryGetValue(baseName, out float start))
                {
                    pairs.Add(new RhythmAction(baseName, start, e.time_ms));
                    stack.Remove(baseName);
                }
            }
        }

        return pairs;
    }
}