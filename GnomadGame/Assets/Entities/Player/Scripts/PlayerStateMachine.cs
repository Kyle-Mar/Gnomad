using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{

    PlayerBaseState currentState;
    PlayerStateFactory states;
    public PlayerControls Controls;

    LayerMask groundLayerMask;
    ContactFilter2D floorContactFilter;
    ContactFilter2D wallContactFilter;

    bool isGrounded = false;
    bool isTouchingWallLeft = false;
    bool isTouchingWallRight = false;

    Vector2 lastMovementDirection = new(0,0);

    float jumpBufferTime = 0f;

    public Collider2D col;
    public Rigidbody2D rb;

    public PlayerBaseState CurrentState { get { return currentState; } set { currentState = value; } }
    public bool IsGrounded { get { return isGrounded; } private set { } }
    public float JumpBufferTime { get { return jumpBufferTime; } private set { } }
    public bool IsTouchingWallLeft { get { return isTouchingWallLeft; } private set { } }
    public bool IsTouchingWallRight { get { return isTouchingWallRight; } private set { } }
    public bool IsTouchingWall { get { return IsTouchingWallLeft || IsTouchingWallRight; } private set { } }
    public Vector2 LastMovementDirection { get { return lastMovementDirection; } private set { } }


    private void OnEnable()
    {
        Controls.Player.Move.performed += UpdateMovementDirection;
        Controls.Enable();
    }
    private void OnDisable()
    {
        Controls.Enable();
    }

    private void Awake()
    {
        groundLayerMask = LayerMask.GetMask("Ground");
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        Controls = new PlayerControls();

        // this may need to be substituted with a sort of raycast+boxcast solution.
        floorContactFilter = new();
        floorContactFilter.SetLayerMask(groundLayerMask);
        floorContactFilter.SetNormalAngle(85, 95);

        //wallContactFilter = new();
        //wallContactFilter.SetLayerMask(groundLayerMask);
        //wallContactFilter.SetNormalAngle(85, 95);

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
        DoJumpBuffer();
        currentState.UpdateStates();
    }

    public void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, col.bounds.extents * 2, 0f, -transform.up, 0.5f, groundLayerMask);
        //Debug.Log(hit.collider.name);
        //Debug.DrawRay(transform.position, -transform.up * col.bounds.extents.y * 2, Color.blue, .5f);

        if (hit.collider && col.IsTouching(hit.collider, floorContactFilter))
        {
            //Debug.Log("TOUCHING LAYERS:" + col.IsTouchingLayers(groundLayerMask));
            //Debug.Log("TOUCHING COLLIDER:" + hit.collider.IsTouching(col));

            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void DoJumpBuffer()
    {
        if (Controls.Player.Jump.WasPressedThisFrame())
        {
            jumpBufferTime = 0.15f;
        }
        else if( jumpBufferTime >= 0)
        {
            jumpBufferTime -= Time.deltaTime;
        }
    }

    public void ConsumeJumpBuffer()
    {
        if(jumpBufferTime >= 0)
        {
            jumpBufferTime = 0f;
        }
    }

    private void UpdateMovementDirection(InputAction.CallbackContext cxt)
    {
        lastMovementDirection = cxt.ReadValue<Vector2>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.transform.tag == "Ground")
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                float angle = Vector2.SignedAngle(Vector2.up, contact.normal);
                angle = Mathf.RoundToInt(angle);

                isTouchingWallRight = (angle == -90f) ? true : false;
                isTouchingWallRight = (angle == 90f) ? true : false;
                
                //angle == 0f i am touching ceiling.
                //angle == 180f i am touching floor.
            }
        }
    }
}
