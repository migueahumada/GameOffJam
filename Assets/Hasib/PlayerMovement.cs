using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 _input;
    [SerializeField] Rigidbody _rb;

    [SerializeField] private float speed;
   // [SerializeField] private float rotationSpeed = 360;
    [SerializeField] Animator _animator;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Look();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public float rotationSpeed = 8f;

    void Look()
    {
        if (_input != Vector3.zero)
        {
            Vector3 dir = -_input;

            // Force direction to stay horizontal
            dir.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(dir);

            // Smooth rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }





    void Move()
    {
        _rb.MovePosition(transform.position + transform.forward * (_input.magnitude * speed * Time.deltaTime));
        if (_input.magnitude > 0)
        {
            _animator.SetBool("isWalking", true);
        }
        else
        {
            _animator.SetBool("isWalking", false);
        }
    }

    void GetInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); 
    }
}