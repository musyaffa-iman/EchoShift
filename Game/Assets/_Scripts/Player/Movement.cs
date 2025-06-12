using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float smoothTime = 0.05f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    public Vector2 MoveInput => moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 targetVelocity = moveInput * speed;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothTime);
    }

    public void SetMovementInput(Vector2 input)
    {
        moveInput = input;
    }
}
