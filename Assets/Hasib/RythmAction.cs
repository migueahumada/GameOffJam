using System;
using System.Collections.Generic;

[System.Serializable]
public class RhythmAction
{
    public String name;
    public Single startTimeMs;
    public Single endTimeMs;
    public Single DurationMs => endTimeMs - startTimeMs;

    public RhythmAction(String name, Single start, Single end)
    {
        this.name = name;
        this.startTimeMs = start;
        this.endTimeMs = end;
    }

    public static List<RhythmAction> PairEvents(List<TimelineEvent> events)
    {
        List<RhythmAction> pairs = new List<RhythmAction>();
        Dictionary<String, Single> stack = new Dictionary<String, Single>();

        foreach (TimelineEvent e in events)
        {
            Single eventTime = e.GetTimeMs(); 

            if (e.eventName.EndsWith("start"))
            {
                String baseName = e.eventName.Replace(" start", "");
                stack[baseName] = eventTime;
            }
            else if (e.eventName.EndsWith("finish"))
            {
                String baseName = e.eventName.Replace(" finish", "");
                if (stack.TryGetValue(baseName, out Single start))
                {
                    pairs.Add(new RhythmAction(baseName, start, eventTime));
                    stack.Remove(baseName);
                }
            }
        }

        return pairs;
    }
}