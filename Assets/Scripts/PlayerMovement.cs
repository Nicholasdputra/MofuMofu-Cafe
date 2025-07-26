using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Disable gravity for top-down movement
        rb.gravityScale = 0f;
        
        // Freeze Z rotation to prevent spinning
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Get input from keyboard
        GetInput();
    }
    
    void FixedUpdate()
    {
        // Apply movement in FixedUpdate for smooth physics-based movement
        rb.velocity = moveInput * moveSpeed;
    }
    
    void GetInput()
    {
        // Get horizontal and vertical input (WASD or Arrow Keys)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Create movement vector
        moveInput = new Vector2(horizontal, vertical).normalized;
    }
}
