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
        if (rb.velocity.magnitude > 0.1f)
        {
            Flip();
        }
    }

    private void Flip()
    {
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
