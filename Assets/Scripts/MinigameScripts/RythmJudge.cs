using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MinigameScripts
{
    public class RhythmJudge : MonoBehaviour
    {
        [Serializable]
        public class RhythmPair
        {
            public float StartTimeMs;
            public float EndTimeMs;
            public bool Processed;
        }

        [Header("Timeline Reference")]
        [SerializeField] public FMODEventTimeline _timeline;

        [Header("Scoring Settings")]
        public float WindowMs = 300f; // Window of 300ms (±150ms)
        public float InputOffsetMs = 0f; 
        public static event Action OnGameOverWin;
        public static event Action OnGameOverLoose;
        public bool inputUpEnabled = false;


        private List<RhythmPair> _rhythmPairs = new List<RhythmPair>();
        private int _currentPairIndex = 0;
        private bool _isHoldingCorrectly = false;
        private int _successfulPairs = 0;
        private int _totalPairs = 0;

        // Events

        public static event Action OnMiss;
        public static event Action OnStartPour;
        public static event Action OnJudgeCreated;
        public static event Action OnPerfectInput;
        public static event Action OnPerfectRelease;

        private void Start()
        {
            OnJudgeCreated?.Invoke();
            while(_timeline == null)
            {
                _timeline = FindFirstObjectByType<FMODEventTimeline>();
            }
            // Initial parse
            ParseTimeline();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            // --- Logic Safety Checks ---
            if (_rhythmPairs.Count == 0 || _currentPairIndex >= _rhythmPairs.Count) return;

            float currentTimeMs = (_timeline.CurrentTimeSeconds * 1000f) + InputOffsetMs;
            var currentPair = _rhythmPairs[_currentPairIndex];

            // 1. TIMEOUT CHECK (Missed by doing nothing)
            // If the time has passed the Start Window and we aren't holding yet -> Missed Start
            if (!_isHoldingCorrectly && currentTimeMs > currentPair.StartTimeMs + (WindowMs / 2f))
            {
                 //Debug.Log($"❌ Missed Note {_currentPairIndex} (Too Slow!)");
                 // miss notification
                 OnMiss?.Invoke();
                 currentPair.Processed = true; // Mark as done so we don't double count
                 AdvanceToNextPair();
                 return;
            }
            
            // If we ARE holding, but passed the End Window -> Missed End
            if (_isHoldingCorrectly && currentTimeMs > currentPair.EndTimeMs + (WindowMs / 2f))
            {
                 //Debug.Log($"❌ Held too long! {_currentPairIndex}");
                 // miss notification (vale la pena diferenciar misses NOT DONE)
                 OnMiss?.Invoke();
                 currentPair.Processed = true;
                 AdvanceToNextPair();
                 return;
            }


            // 2. INPUT DOWN (Start Hold)
            if (keyboard.spaceKey.wasPressedThisFrame && !currentPair.Processed)
            {
                float diff = currentTimeMs - currentPair.StartTimeMs;
                
                // Check if inside window
                if (Mathf.Abs(diff) <= (WindowMs / 2f))
                {
                    OnStartPour?.Invoke();
                    _isHoldingCorrectly = true;
                    OnPerfectInput?.Invoke();
                    if (!inputUpEnabled)
                    {
                        _successfulPairs++;
                    } 
                    //Debug.Log($"✅ CAUGHT START! (Diff: {diff:F0}ms)");
                }
                else
                {
                    // --- CRITICAL FIX IS HERE ---
                    if (diff < 0)
                    {
                        if (Mathf.Abs(diff) < 600.00f)
                        {
                            OnMiss?.Invoke();
                            Debug.Log($"⚠️ Too Early! Wait! (Early by {Mathf.Abs(diff):F0}ms)");
                            AdvanceToNextPair();
                        }
                        Debug.Log(Mathf.Abs(diff));
                        // Negative diff means we are EARLY (Time < StartTime)
                        // miss notification?????????????????????? si se destruye ya no puede probar a darle otra vez. solo sonido?
                        // DO NOT ADVANCE! Let the player try again or wait for the note.
                    }
                    else
                    {
                        // Positive diff means we are LATE (Time > StartTime)
                        Debug.Log($"❌ Too Late! (Late by {diff:F0}ms)");
                        // miss notification
                        OnMiss?.Invoke();
                        // If we are late, we missed it. Skip to next.
                        AdvanceToNextPair();
                    }
                }
            }

            // 3. INPUT UP (Release Hold)
            if (keyboard.spaceKey.wasReleasedThisFrame)
            {
                if (_isHoldingCorrectly)
                {
                    float diff = currentTimeMs - currentPair.EndTimeMs;

                    if (Mathf.Abs(diff) <= (WindowMs / 2f))
                    {
                        if (inputUpEnabled)
                        {
                            _successfulPairs++;
                        }  
                        OnPerfectRelease?.Invoke();
                        //Debug.Log($"✨ PERFECT RELEASE! (Diff: {diff:F0}ms)");
                    }
                    else
                    {
                        if (inputUpEnabled)
                        {
                            // miss notification
                            Debug.Log($"❌ Released Badly (Diff: {diff:F0}ms)");
                            OnMiss?.Invoke();    
                        }
                    }
                    
                    // Whether good or bad release, this note is done.
                    currentPair.Processed = true;
                    AdvanceToNextPair();
                }
                else
                {
                    // If we released but weren't holding correctly, ignore it.
                    // (Or reset holding state just in case)
                    _isHoldingCorrectly = false;
                }
            }
        }

        private void ParseTimeline()
        {
            if (_timeline == null || _timeline.Events.Count == 0) return;

            _rhythmPairs.Clear();
            var sortedEvents = _timeline.Events.OrderBy(e => e.GetTimeMs()).ToList();

            for (int i = 0; i < sortedEvents.Count; i++)
            {
                if (sortedEvents[i].eventName.Trim() == "input down")
                {
                    for (int j = i + 1; j < sortedEvents.Count; j++)
                    {
                        if (sortedEvents[j].eventName.Trim() == "input up")
                        {
                            _rhythmPairs.Add(new RhythmPair
                            {
                                StartTimeMs = sortedEvents[i].GetTimeMs(),
                                EndTimeMs = sortedEvents[j].GetTimeMs()
                            });
                            i = j;
                            break;
                        }
                    }
                }
            }
            _totalPairs = _rhythmPairs.Count;
            Debug.Log($"Judge: Parsed {_totalPairs} pairs.");
        }


        private void AdvanceToNextPair()
        {
            _currentPairIndex++;
            _isHoldingCorrectly = false;
            if (_currentPairIndex >= _totalPairs)
            {
                float score = _totalPairs > 0 ? ((float)_successfulPairs / _totalPairs) * 100f : 0;
                Debug.Log($"Score: {score}");
                if (score >= 65) OnGameOverWin?.Invoke();
                else OnGameOverLoose?.Invoke();         
            }
        }
    }
}