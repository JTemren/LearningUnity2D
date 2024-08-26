using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Player Variables")] [Space(10)]
    
    [Header("Components")]
    
    public Rigidbody2D rigidbody2D;
    [Space(10)] 
    
    [Header("Gravity")] 
    [SerializeField] private float baseGravity = 2f;
    [SerializeField] private float maxFallSpeed = 18;
    [SerializeField] private float fallSpeedMultiplier = 2f;
    
    [Header("Locomotion")]
    [SerializeField][Range(0,10f)] private float moveSpeed = 5f;
    [SerializeField] private float horizontalMovement;
    [Space(10)]
    
    [Header("Jump")]
    [SerializeField] private bool isGrounded;
    [SerializeField][Range(0,40f)]private float jumpPower = 10f;
    [Space(20)]
    
    // Double jump Variables
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private int jumpsRemaining;
    [Space(20)]
    
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.2f,0.2f);
    [SerializeField] private LayerMask groundLayer;
    
    
    // Awake is called on initialization
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        groundCheckPos = transform.Find("GroundCheckPos");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = false;
        GroundCheck();
        Gravity();



    }
    // FixedUpdate is called once per fixedDeltaTime interval (default of 50 times per second)
    private void FixedUpdate()
    {
        Locomotion();
        
    }

    // LateUpdate is called after update
    private void LateUpdate()
    {
        
    }
    
    private void Locomotion()
    {
        rigidbody2D.velocity = new Vector2(horizontalMovement * moveSpeed, rigidbody2D.velocity.y);
    }

    private void Gravity()
    {
        if (rigidbody2D.velocity.y < 0 )
        {
            rigidbody2D.gravityScale = baseGravity * fallSpeedMultiplier;
            rigidbody2D.velocity =
                new Vector2(rigidbody2D.velocity.x, Mathf.Max(rigidbody2D.velocity.y, -maxFallSpeed));
        }
        else
        {
            rigidbody2D.gravityScale = 2f;
        }
    }

    private void GroundCheck()
    {
        if (!Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer)) return;
        isGrounded = true;
        jumpsRemaining = maxJumps;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // TODO find "good" feeling values for gravity
        
        if (jumpsRemaining <= 0) return;
        if (context.performed)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpPower);
            jumpsRemaining--;
        }
        if (context.canceled && rigidbody2D.velocity.y > 0f)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y * 0.5f);
            jumpsRemaining--;
        }
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.red : Color.white;
        Gizmos.DrawCube(groundCheckPos.position,groundCheckSize);
    }
}
