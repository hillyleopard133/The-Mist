using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.UI;

public class HamsterController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float walkSpeed;
    
    public float WalkSpeed  => walkSpeed;

    private PlayerActions actions;
    private Rigidbody2D rb2D;
    private float horizontalInput;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.6f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    
    [SerializeField] private float jumpForce = 12f;
    
    private void Awake()
    {
        actions = new PlayerActions();
        rb2D = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        actions.Hamster.Jump.performed += ctx => Jump();
        
        ReadMovement();
        CheckGrounded();
    }    
    
    private void FixedUpdate()
    {
        Move();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer
        );
    }

    private void Jump()
    {
        if (!isGrounded) return;

        rb2D.linearVelocity = new Vector2(
            rb2D.linearVelocity.x,
            jumpForce
        );
    }
    
    private void Move()
    {
        rb2D.linearVelocity = new Vector2(horizontalInput * walkSpeed, rb2D.linearVelocity.y);
    }

    private void ReadMovement()
    {
        horizontalInput = actions.Hamster.Move.ReadValue<Vector2>().x;
        //if(horizontalInput == Vector2.zero)
        {
            //playerAnimations.SetMoveBoolTransition(false);
            return;
        }
        
        //playerAnimations.SetMoveBoolTransition(true);
        //playerAnimations.SetMoveAnimation(moveDirection);
    }
    
    public void EnableMovement()
    {
        actions.Hamster.Move.Enable();
        actions.Hamster.Jump.Enable();
    }

    public void DisableMovement()
    {
        actions.Hamster.Move.Disable();
        actions.Hamster.Jump.Disable();
    }

    private void OnEnable()
    {
        actions.Hamster.Enable();
    }

    private void OnDisable()
    {
        actions.Hamster.Disable();
    }
}
