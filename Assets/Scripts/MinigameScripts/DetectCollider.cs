using MinigameScripts;
using UnityEngine;
using Unity.VisualScripting;
namespace MinigameScripts

{
    public class DetectCollider : MonoBehaviour
    {
        private bool cupIn = false;
        public CoffeeMachineController coffeeMachineController;


        // Almacenamos el componente Animator para poder usarlo después.
        public Animator objetivoAnimator;

        private void OnEnable()
        {
            CoffeeMachineController.OnPrepStart += HandlePrepStart;
        }

        private void OnDisable()
        {
            CoffeeMachineController.OnPrepStart -= HandlePrepStart;
        }

        private void HandlePrepStart()
        {
            Debug.Log(cupIn);
            if (cupIn) {
                objetivoAnimator?.SetTrigger("CupIntro");
                //Debug.Log("prepstart from detect collider");
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // 1. Intentamos obtener el componente Animator del objeto que entró.
            if (other.gameObject.name == "coffee_M")
            {
                objetivoAnimator = other.transform.GetComponentInParent<Animator>();
            }
            else if (other.gameObject.name == "S Cup(Clone)" || other.gameObject.name == "XS Cup(Clone)")
            {
                objetivoAnimator = other.GetComponent<Animator>();
            }

            if (objetivoAnimator != null)cupIn = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            // Si el objeto sale, es una buena práctica liberar la referencia.
            if (other.GetComponent<Animator>() == objetivoAnimator)
            {
                objetivoAnimator = null;
                cupIn = false;
            }
        }
    }
}