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
            if (cupIn && objetivoAnimator != null && objetivoAnimator.isActiveAndEnabled)
            {
                objetivoAnimator.SetTrigger("Miss");
            }
            else
            {
                Debug.LogWarning("HandleMiss: No hay Animator válido para disparar la animación.");
            }
        }

        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Cup")
            {
                // Intentamos obtener el Animator en el objeto
                if (!other.gameObject.TryGetComponent<Animator>(out objetivoAnimator))
                {
                    // Si falla (devuelve false), buscamos en el padre (error de maquetado en el juego un poco chapuza)
                    objetivoAnimator = other.gameObject.GetComponentInParent<Animator>();
                }
                cupIn = true;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            // Si el objeto sale, es una buena práctica liberar la referencia.
            if (other.GetComponent<Animator>() == objetivoAnimator)
            {
                cupIn = false;
                objetivoAnimator = null;
            }
        }
    }
}