using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class RhythmJudge : MonoBehaviour
{
    [Header("Referencia al timeline FMOD")]
    public FMODEventTimeline timeline;

    [Header("Configuración de precisión")]
    [Tooltip("Ventana de tiempo en milisegundos donde se considera el input válido (± windowMs / 2)")]
    public float windowMs = 200f;

    [Tooltip("Compensación en milisegundos para corregir la latencia entre FMOD y Unity")]
    public float inputOffsetMs = -200f; // 🔧 Ajusta en Inspector (negativo = adelanta input)

    private float keyDownTime = 0f;
    private float keyUpTime = 0f;

    void Update()
    {
        if (timeline == null || timeline.Events == null || timeline.Events.Count == 0)
            return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Detectar pulsación
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            keyDownTime = (timeline.CurrentTimeSeconds * 1000f) + inputOffsetMs;
            EvaluateTiming(keyDownTime, "KeyDown");
        }

        // Detectar liberación
        if (keyboard.spaceKey.wasReleasedThisFrame)
        {
            keyUpTime = (timeline.CurrentTimeSeconds * 1000f) + inputOffsetMs;
            EvaluateTiming(keyUpTime, "KeyUp");

            var nearest = GetNearestEvent(keyDownTime);
            if (nearest != null && keyUpTime < nearest.time_ms)
                Debug.Log($"🔸 Soltaste antes de que terminara el evento '{nearest.eventName}'");
        }
    }

    void EvaluateTiming(float inputTimeMs, string action)
    {
        var nearest = GetNearestEvent(inputTimeMs);
        if (nearest == null) return;

        float diff = inputTimeMs - nearest.time_ms;
        string relation = diff < 0 ? "⏪ Antes" : "⏩ Después";
        float absDiff = Mathf.Abs(diff);
        float precision = Mathf.Clamp01(1f - (absDiff / (windowMs / 2f))) * 100f;

        Debug.Log($"{action} respecto a '{nearest.eventName}': {relation} por {absDiff:F1}ms | Precisión: {precision:F1}%");
    }

    TimelineEvent GetNearestEvent(float timeMs)
    {
        return timeline.Events
            .OrderBy(e => Mathf.Abs(e.time_ms - timeMs))
            .FirstOrDefault();
    }
}
