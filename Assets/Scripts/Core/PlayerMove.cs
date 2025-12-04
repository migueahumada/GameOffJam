using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody rb;
    public float moveSpeed;
    public float rotationSpeed = 720;
    private Vector2 _moveDirection;
    private Vector3 movementDirection;
    public InputActionReference move;

    public Animator animator;


    // Update is called once per frame
    void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();
        movementDirection = new Vector3(_moveDirection.x*-1, 0, _moveDirection.y*-1);
    
        rb.linearVelocity = movementDirection * moveSpeed * 60 * Time.deltaTime;

        if (movementDirection != Vector3.zero)
        {
            animator.SetBool("Walking", true);
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed*Time.deltaTime);
        }
        else animator.SetBool("Walking", false);
    }
}
