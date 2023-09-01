using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{

    PlayerBaseState currentState;
    PlayerStateFactory states;

    LayerMask groundLayerMask;
    bool isGrounded = false;

    public Collider2D col;
    public Rigidbody2D rb;

    public PlayerBaseState CurrentState { get { return currentState; } set { currentState = value; } }
    public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; } }
    

    private void Awake()
    {
        groundLayerMask = LayerMask.GetMask("Ground");
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        states = new(this);
        currentState = states.Grounded();
        currentState.EnterState();
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckIfGrounded();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentState);
        //CheckIfGrounded();
        currentState.UpdateStates();
    }

    public void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, col.bounds.extents * 2, 0f, -transform.up, 0.5f, groundLayerMask);
        //Debug.Log(hit.collider.name);
        //Debug.DrawRay(transform.position, -transform.up * col.bounds.extents.y * 2, Color.blue, .5f);
        

        if (hit.collider && col.IsTouchingLayers(groundLayerMask))
        {
            Debug.Log("TOUCHING LAYERS:" + col.IsTouchingLayers(groundLayerMask));
            Debug.Log("TOUCHING COLLIDER:" + hit.collider.IsTouching(col));

            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
