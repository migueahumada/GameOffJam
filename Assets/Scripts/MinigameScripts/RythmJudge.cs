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


        [Header("Visuals")] 
        //[SerializeField] private Animator _armAnimator;
        public Animator _armAnimator;
        //[SerializeField] private Animator _machineAnimator;
        public Animator _machineAnimator;

        [Header("Timeline Reference")]
        [SerializeField] public FMODEventTimeline _timeline;

        [Header("Scoring Settings")]
        public float WindowMs = 300f; // Window of 300ms (±150ms)
        public float InputOffsetMs = 0f; 

        private GameObject _gameOverWin;
        private GameObject _gameOverLoose;

        private List<RhythmPair> _rhythmPairs = new List<RhythmPair>();
        private int _currentPairIndex = 0;
        private bool _isHoldingCorrectly = false;
        private int _successfulPairs = 0;
        private int _totalPairs = 0;
        private bool pauseMenu = false;

        public static event Action OnMiss;
        public static event Action OnStartPour;
        public static event Action OnPause;

        private static readonly int Pressed = Animator.StringToHash("Pressed");

        private void Start()
        {
            while(_timeline == null)
            {
                _timeline = FindFirstObjectByType<FMODEventTimeline>();
            }
            _armAnimator = GameObject.FindGameObjectWithTag("Arm").GetComponentInChildren<Animator>();
            _machineAnimator = GameObject.FindGameObjectWithTag("Machine").GetComponent<Animator>();
            _gameOverWin = GameObject.FindGameObjectWithTag("GameOverWin");
            _gameOverLoose = GameObject.FindGameObjectWithTag("GameOverLoose");
            // Initial parse
            ParseTimeline();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            // -- Menu Pausa
            if (keyboard.escapeKey.wasPressedThisFrame) 
            {
                OnPause?.Invoke();
                if (pauseMenu) _armAnimator.enabled = false;
                else _armAnimator.enabled = true;
                pauseMenu = !pauseMenu;
            }
            // --- Visuals Only ---
            if (keyboard.spaceKey.wasPressedThisFrame) 
            {
                _armAnimator.SetBool(Pressed, true);
                _machineAnimator.SetBool(Pressed, true);
            }
            if (keyboard.spaceKey.wasReleasedThisFrame)
            {
                _armAnimator.SetBool(Pressed, false);
                _machineAnimator.SetBool(Pressed, false);
            }    

            // --- Logic Safety Checks ---
            if (_rhythmPairs.Count == 0 || _currentPairIndex >= _rhythmPairs.Count) return;

            float currentTimeMs = (_timeline.CurrentTimeSeconds * 1000f) + InputOffsetMs;
            var currentPair = _rhythmPairs[_currentPairIndex];

            // 1. TIMEOUT CHECK (Missed by doing nothing)
            // If the time has passed the Start Window and we aren't holding yet -> Missed Start
            if (!_isHoldingCorrectly && currentTimeMs > currentPair.StartTimeMs + (WindowMs / 2f))
            {
                 Debug.Log($"❌ Missed Note {_currentPairIndex} (Too Slow!)");
                 _armAnimator.SetTrigger("Miss");
                 // miss notification
                 OnMiss?.Invoke();
                 SFXManager.instance.PlaySFX(7);
                 _armAnimator.SetBool(Pressed, false);
                 _machineAnimator.SetBool(Pressed, false);
                 currentPair.Processed = true; // Mark as done so we don't double count
                 AdvanceToNextPair();
                 return;
            }
            
            // If we ARE holding, but passed the End Window -> Missed End
            if (_isHoldingCorrectly && currentTimeMs > currentPair.EndTimeMs + (WindowMs / 2f))
            {
                 Debug.Log($"❌ Held too long! {_currentPairIndex}");
                 _armAnimator.SetTrigger("Miss");
                 // miss notification
                 OnMiss?.Invoke();
                 SFXManager.instance.PlaySFX(7);
                 _armAnimator.SetBool(Pressed, false);
                 _machineAnimator.SetBool(Pressed, false);
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
                    SFXManager.instance.PlaySFX(5);
                    _isHoldingCorrectly = true;
                    
                    Debug.Log($"✅ CAUGHT START! (Diff: {diff:F0}ms)");
                }
                else
                {
                    // --- CRITICAL FIX IS HERE ---
                    if (diff < 0)
                    {
                        // Negative diff means we are EARLY (Time < StartTime)
                        Debug.Log($"⚠️ Too Early! Wait! (Early by {Mathf.Abs(diff):F0}ms)");
                        SFXManager.instance.PlaySFX(7);
                        _armAnimator.SetTrigger("Miss");
                        // miss notification
                        OnMiss?.Invoke();
                        _armAnimator.SetBool(Pressed, false);
                        _machineAnimator.SetBool(Pressed, false);
                        // DO NOT ADVANCE! Let the player try again or wait for the note.
                    }
                    else
                    {
                        // Positive diff means we are LATE (Time > StartTime)
                        Debug.Log($"❌ Too Late! (Late by {diff:F0}ms)");
                        SFXManager.instance.PlaySFX(7);
                        _armAnimator.SetTrigger("Miss");
                        // miss notification
                        OnMiss?.Invoke();
                        _armAnimator.SetBool(Pressed, false);
                        _machineAnimator.SetBool(Pressed, false);
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
                        SFXManager.instance.PlaySFX(6);
                        _successfulPairs++;
                        Debug.Log($"✨ PERFECT RELEASE! (Diff: {diff:F0}ms)");
                    }
                    else
                    {
                        Debug.Log($"❌ Released Badly (Diff: {diff:F0}ms)");
                        SFXManager.instance.PlaySFX(7);
                        _armAnimator.SetTrigger("Miss");
                        // miss notification
                        OnMiss?.Invoke();
                        _armAnimator.SetBool(Pressed, false);
                        _machineAnimator.SetBool(Pressed, false);
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

        private void showWin()
        {
            _gameOverWin.SetActive(true);
        }
        private void showLoose()
        {
            _gameOverLoose.SetActive(true);
        }
        private void AdvanceToNextPair()
        {
            _currentPairIndex++;
            _isHoldingCorrectly = false;
            
            if (_currentPairIndex >= _totalPairs)
            {
                float score = _totalPairs > 0 ? ((float)_successfulPairs / _totalPairs) * 100f : 0;
                Debug.Log($"🏁 GAME OVER! Score: {score:F1}%");
                if (score >= 60) Invoke("showWin", 2);
                else Invoke("showLoose", 2);                
            }
        }
    }
}