using System;
using FMODUnity;
using UnityEngine;

namespace MinigameScripts
{
    public class CoffeeMachineController : MonoBehaviour
    {
        [SerializeField] private GameObject[] _cups;

        [Header("PREP SOUNDS")]
        [SerializeField] private EventReference prepStart;
        [SerializeField] private EventReference prepFinish;

         private FMODEventTimeline _fmodTimeline;
        //private Boolean _isPouring;
        //animation triggers
        public static event Action OnPrepStart;
        public static event Action OnPrepFinish;
        


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
                _fmodTimeline = FindFirstObjectByType<FMODEventTimeline>();
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

            }
            else if (isFinish)
            {
                Debug.Log("PREP FINISH");
                OnPrepStart?.Invoke();
                RuntimeManager.PlayOneShot(prepFinish);
            }


            if (evtName.Contains("input up"))
            {
                // PREP CUP OUT
                OnPrepFinish?.Invoke();
                
                //StartPour();
                //notify start input
            }
            else if (evtName.Contains("input down"))
            {
                
                
                //StopPour();

            }
        }

        private void HandleCupEntry(String eventName)
        {
            RuntimeManager.PlayOneShot(prepStart);
            if (eventName.Contains("2c norm")) 
            {    
                GameObject sCupInstance = Instantiate(_cups[1]); 
            }
            else if (eventName.Contains("2c fast"))
            {
                GameObject xsCupInstance = Instantiate(_cups[2]);   
            }
            else
            {
                GameObject xsCupInstance = Instantiate(_cups[0]);    
            } 
        }

        //-- private void StartPour()
        //{
        //    if (_pourStream) _pourStream.SetActive(true);
        //    _isPouring = true;
        //}

        //private void StopPour()
        //{
        //    if (_pourStream) _pourStream.SetActive(false);
        //    _isPouring = false;
        //} _
    }
}