using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float startJumpTime;
    [SerializeField] float maxJumpTime;
    
    [SerializeField] BoxCollider2D col;
    [SerializeField] Rigidbody2D rb;
    
    [SerializeField] bool isJumping = false;
    [SerializeField] bool isGrounded = false;
    
    private void Start()
    {
        groundLayerMask = LayerMask.GetMask("Ground");
        col = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Jump();
        Move();
    }

    void Jump()
    {
        IsGrounded();
        // We must ensure that we are grounded and not jumping before we begin to jump.
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && isGrounded)
        {
            isJumping = true;
            // set up future time that we will stop jumping unless the player stops first.
            startJumpTime = Time.time;
            maxJumpTime = startJumpTime + MovementStats.maxJumpHeight;
            rb.velocity = new Vector2(rb.velocity.x, MovementStats.jumpSpeed);
        }
        // while the player is holding the spacebar.
        // continue jumping as long as the timer will allow us.
        else if (Input.GetKey(KeyCode.Space) && isJumping && (maxJumpTime > Time.time))
        {
            rb.AddForce(Vector2.up * MovementStats.jumpSpeed);
        }
        else
        {
            // TODO: Implement non-frame independent lerp later
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(rb.velocity.x, MovementStats.fallSpeed), Time.deltaTime);
            isJumping = false;
        }
    }

    void Move()
    {
        Vector2 movementVector = new(0,0);
        if (Input.GetKey(KeyCode.A))
        {
            movementVector.x -= MovementStats.moveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementVector.x += MovementStats.moveSpeed;
        }
        movementVector.y = rb.velocity.y;

        rb.velocity = movementVector;
    }

    void IsGrounded()
    {
        // Check if the player is grounded with a boxcast instead of a raycast.
        // If necessary it can be changed to 2 raycasts on the bottom corners. 
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, col.bounds.extents*2, 0f, -transform.up, 0.5f, groundLayerMask);
        if(hit.collider && col.IsTouchingLayers(groundLayerMask))
        {
            Debug.Log("test");
            isGrounded = true;
        }else 
        {
            isGrounded = false;
        }
    }
}
