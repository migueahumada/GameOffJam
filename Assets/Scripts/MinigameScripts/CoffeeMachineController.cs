using System;
using UnityEngine;

namespace MinigameScripts
{
    public class CoffeeMachineController : MonoBehaviour
    {
        [Header("Animators")]
        [SerializeField] private Animator _machineAnimator;
        [SerializeField] private Animator _cupAnimator;

        [Header("Pour Effect")]
        [SerializeField] private GameObject _pourStream;
        
        [Header("FMOD Timeline")]
        [SerializeField] private FMODEventTimeline _fmodTimeline;

        private Boolean _isPouring;

        private static readonly Int32 CupIntro = Animator.StringToHash("CupIntro");
        private static readonly Int32 CupOutro = Animator.StringToHash("CupOutro");
        private static readonly Int32 MachinePress = Animator.StringToHash("MachinePress");
        private static readonly Int32 AnimSpeed = Animator.StringToHash("Speed");

        private void OnEnable()
        {
            FMODEventTimeline.OnTimelineEventTriggered += HandleTimelineEvent;
        }

        private void OnDisable()
        {
            FMODEventTimeline.OnTimelineEventTriggered -= HandleTimelineEvent;
        }

        private void Start()
        {
            if (_fmodTimeline == null)
                _fmodTimeline = FindObjectOfType<FMODEventTimeline>();

            if (_pourStream) _pourStream.SetActive(false);
        }

        private void HandleTimelineEvent(TimelineEvent evt)
        {
            String evtName = evt.eventName.ToLower();
            Boolean isStart = evtName.EndsWith("start");
            Boolean isFinish = evtName.EndsWith("finish");

            if (isStart)
            {
                if (evtName.Contains("prep"))
                {
                    HandleCupEntry(evtName);
                }
                else if (evtName.Contains("input"))
                {
                    StartPour();
                }
            }
            else if (isFinish)
            {
                if (evtName.Contains("input"))
                {
                    StopPour();
                    
                    _cupAnimator.SetTrigger(CupOutro);
                }
            }
        }

        private void HandleCupEntry(String eventName)
        {
            Single speedMultiplier = 1f;

            if (eventName.Contains("1c")) speedMultiplier = 4f; 
            else if (eventName.Contains("2c")) speedMultiplier = 2f;
            else speedMultiplier = 1f;

            _cupAnimator.SetFloat(AnimSpeed, speedMultiplier);
            _cupAnimator.SetTrigger(CupIntro);
        }

        private void StartPour()
        {
            _machineAnimator.SetTrigger(MachinePress);
            if (_pourStream) _pourStream.SetActive(true);
            _isPouring = true;
        }

        private void StopPour()
        {
            _machineAnimator.SetTrigger(MachinePress);
            if (_pourStream) _pourStream.SetActive(false);
            _isPouring = false;
        }
    }
}