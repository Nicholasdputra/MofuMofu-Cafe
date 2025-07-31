using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        // Check if the NPC is moving
        if (rb.velocity.magnitude > 0.1f)
        {
            // If moving, flip the sprite based on direction
            Flip();
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
