using MinigameScripts;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
namespace MinigameScripts

{
    public class DetectCollider2 : MonoBehaviour
    {
        private bool cupIn = false;
        //private bool isPouring = false;
        public CoffeeMachineController coffeeMachineController;
        
        public RhythmJudge rythmJudge;
        


        // Almacenamos el componente Animator para poder usarlo después.
        public Animator objetivoAnimator;
        public GameObject selfCup;

        void Start()
        {

        }

        private void HandleCreated()
        {
            rythmJudge = FindAnyObjectByType<RhythmJudge>();
        }

        private void OnEnable()
        {
            CoffeeMachineController.OnPrepFinish += HandlePrepFinish;
            RhythmJudge.OnStartPour += HandleStartPour;
            RhythmJudge.OnMiss += HandleMiss;
            RhythmJudge.OnJudgeCreated += HandleCreated;
        }

        private void OnDisable()
        {
            CoffeeMachineController.OnPrepFinish -= HandlePrepFinish;
            RhythmJudge.OnMiss -= HandleMiss;
            RhythmJudge.OnStartPour -= HandleStartPour;
            RhythmJudge.OnJudgeCreated -= HandleCreated;
        }

        private void HandleStartPour()
        {
            //isPouring = true;
            objetivoAnimator?.SetBool("isPouring", true);
        }
        private void HandlePrepFinish()
        {
            if (cupIn) 
            {
                objetivoAnimator?.SetTrigger("CupOutro");

            }
        }

        private void HandleMiss()
        {
            //isPouring = false;
            if (cupIn) 
            {
                objetivoAnimator?.SetTrigger("Miss");
                //objetivoAnimator?.SetBool("isPouring", false);
            }
        }

        
        private void OnTriggerEnter(Collider other)
        {
            // 1. Intentamos obtener el componente Animator del objeto que entró.
            if (other.gameObject.name == "coffee_M")
            {
                selfCup = other.gameObject.transform.parent.gameObject;
                objetivoAnimator = other.transform.GetComponentInParent<Animator>();
            }
            else if (other.gameObject.name == "S Cup(Clone)" || other.gameObject.name == "XS Cup(Clone)")
            {
                selfCup = other.gameObject;
                objetivoAnimator = other.GetComponent<Animator>();
            }

            if (objetivoAnimator != null) cupIn = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            // Si el objeto sale, es una buena práctica liberar la referencia.
            if (other.GetComponent<Animator>() == objetivoAnimator)
            {
                cupIn = false;
                objetivoAnimator = null;
                Debug.Log("Objeto con Animator salió del área de detección.");
            }
        }
    }
}