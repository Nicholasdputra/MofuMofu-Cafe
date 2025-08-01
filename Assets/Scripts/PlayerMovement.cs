using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public bool canMove;

    [Header("Interactable Trigger Area")]
    [SerializeField] private BoxCollider2D interactableTriggerArea;
    [SerializeField] private float colliderOffsetDistance = 1f;
    private float lastMoveX;

    [Header("Sprite")]

    [SerializeField] private SpriteRenderer sr;
    Animator animator;

    void Start()
    {
        canMove = true; // Allow movement by default
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        // Disable gravity for top-down movement
        rb.gravityScale = 0f;
        // Freeze Z rotation to prevent spinning
        rb.freezeRotation = true;
        // Ensure the interactable trigger area is set up correctly
        interactableTriggerArea.isTrigger = true;
    }

    void Update()
    {
        // Get input from keyboard
        GetInput();
        // Update collider offset based on movement
        UpdateColliderOffset();
    }

    void FixedUpdate()
    {
        // Apply movement in FixedUpdate for smooth physics-based movement
        if (canMove)
        {
            rb.velocity = moveInput * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop movement if cannot move
        }
        animator.SetFloat("Velocity", rb.velocity.magnitude);
        if (rb.velocity.magnitude > 0.1f)
        {
            // If moving, flip the sprite based on direction
            Flip();
        }
    }

    void GetInput()
    {
        // Get horizontal and vertical input (WASD or Arrow Keys)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Create movement vector
        moveInput = new Vector2(horizontal, vertical).normalized;

        // Update last move direction if there's input
        if (moveInput != Vector2.zero)
        {
            lastMoveX = moveInput.x;
        }
    }

    void UpdateColliderOffset()
    {
        // Only update offset if we have a last move direction
        if (lastMoveX != 0)
        {
            // Calculate offset position based on last movement direction
            Vector2 offset = new Vector2(lastMoveX, 0) * colliderOffsetDistance;
            interactableTriggerArea.offset = offset;
        }
    }
    
    private void Flip()
    {
        // Flip the NPC's sprite based on movement direction
        if (rb.velocity.x > 0)
        {
            sr.flipX = false; // Facing right
        }
        else if (rb.velocity.x < 0)
        {
            sr.flipX = true; // Facing left
        }
    }
}
