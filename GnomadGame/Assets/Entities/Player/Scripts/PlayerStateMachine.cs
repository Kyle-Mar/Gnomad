using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerStateMachine : StateMachine
{
    [Header("States")]

    public PlayerGroundedState GroundedState;
    public PlayerFallState FallState;
    public PlayerGroundPoundState GroundPoundState;
    public PlayerIdleState IdleState;
    public PlayerRunState RunState;
    public PlayerSlideState SlideState;
    public PlayerJumpState JumpState;
    public PlayerWallJumpState WallJumpState;
    public PlayerWallSlideState WallSlideState;
    public PlayerEmptyState EmptyState;

    PlayerBaseState currentState;//Trouble
    public PlayerControls Controls;

    LayerMask groundLayerMask;
    ContactFilter2D floorContactFilter;
    ContactFilter2D wallContactFilter;

    [Header("Movement")]

    [SerializeField] bool isGrounded = false;
    [SerializeField] bool isTouchingWallLeft = false;
    [SerializeField] bool isTouchingWallRight = false;
    [SerializeField] bool wallSlideExpired = false;

    [SerializeField] Vector2 lastMovementDirection = new(0,0);

    [SerializeField] float jumpBufferTime = 0f;
    [SerializeField] float currentMoveSpeed = MovementStats.moveSpeed;

    [Header("Components")]

    public Collider2D col;
    public Rigidbody2D rb;
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer HatSpriteRenderer;
    public PhysicsMaterial2D sticky;
    public PhysicsMaterial2D slippery;
    public GameObject DeathPartsPrefab;
    public Camera Cam;
    public Transform Feet;

    [Header("Zone Dependent Assets")]
    public ParticleSystem WalkParticles;
    public ParticleSystem JumpCloudParticles;
    public ParticleSystem LandParticles;

    override public PlayerBaseState CurrentState { get { return currentState; } set { currentState = value; } }//Trouble


    public bool IsGrounded => isGrounded;
    public float JumpBufferTime => jumpBufferTime;
    public bool IsTouchingWallLeft => isTouchingWallLeft;
    public bool IsTouchingWallRight => isTouchingWallRight;
    public bool IsTouchingWall => isTouchingWallLeft || isTouchingWallRight;
    public bool WallSlideExpired => wallSlideExpired;
    public Vector2 LastMovementDirection => lastMovementDirection;
    public float CurrentMoveSpeed => currentMoveSpeed;



    private void OnEnable()
    {
        Controls.Player.Move.performed += UpdateMovementDirection;
        Controls.Player.Die.performed += Die;
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
        currentMoveSpeed = MovementStats.moveSpeed;
        Cam = GetComponent<Camera>();

        //spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //hatSpriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();

        Assert.IsNotNull(SpriteRenderer);
        Assert.IsNotNull(HatSpriteRenderer);
        Assert.IsNotNull(DeathPartsPrefab);
        Assert.IsNotNull(Feet);


        Controls = new PlayerControls();

        // this may need to be substituted with a sort of raycast+boxcast solution.
        floorContactFilter = new();
        floorContactFilter.SetLayerMask(groundLayerMask);
        floorContactFilter.SetNormalAngle(85, 95);

        //wallContactFilter = new();
        //wallContactFilter.SetLayerMask(groundLayerMask);
        //wallContactFilter.SetNormalAngle(85, 95);

        InstantiateStates();
            
        currentState = GroundedState;
        currentState.EnterState();
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckIfGrounded();
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        currentState.FixedUpdateStates();
    }

    void Update()
    {
        CheckIfGrounded();
        DoJumpBuffer();
        currentState.UpdateStates();

        if (true)//currentState != GroundedState)
        {
            Debug.Log("Current State: " + currentState);
        }
    }

    public void CheckIfGrounded()
    {
        if (ContextUtils.CheckIfGrounded(ref col, transform, ref floorContactFilter))
        {
            isGrounded = true;
            wallSlideExpired = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void SetMoveSpeed(float value)
    {
        currentMoveSpeed = value;
    }

    public void SetWallSlideExpired ( bool value)
    {
        wallSlideExpired = value;
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
       Vector2 inputVector = cxt.ReadValue<Vector2>();

        if (inputVector == Vector2.left)
        {
            lastMovementDirection = inputVector;
        }
        else if (inputVector == Vector2.right)
        {
            lastMovementDirection = inputVector;

        }
    }

    public bool DoWallSlide()
    {
        CheckIfGrounded();
        if (IsGrounded)
        {
            return false;
        }

        if (wallSlideExpired)
        {
            return false;
        }

        float inputX = Controls.Player.Move.ReadValue<Vector2>().x;
        if (IsTouchingWallLeft && inputX < 0)
        {
            return true;
        }
        if (IsTouchingWallRight && inputX > 0)
        {
            return true;
        }
        return false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.transform.tag == "Ground")
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                float angle = Vector2.SignedAngle(Vector2.up, contact.normal);
                angle = Mathf.RoundToInt(angle);

                //Debug.Log(angle);

                isTouchingWallLeft = (angle == -90f) ? true : false;
                isTouchingWallRight = (angle == 90f) ? true : false;
                
                //angle == 0f i am touching ceiling.
                //angle == 180f i am touching floor.
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.transform.tag == "Ground")
        {
            isTouchingWallLeft = false;
            isTouchingWallRight = false;
        }
    }

    public void FlipSprite()
    {
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
        HatSpriteRenderer.flipX = !HatSpriteRenderer.flipX;
        if (!SpriteRenderer.flipX)
        {
        //flip hinge joint angles
        }
    }

    public void Die(InputAction.CallbackContext cxt)
    {//this is a temperary function mostly for shits &/Or giggles
     //this should probably be it's own state and this code is not 
     //necessarily great
        HatSpriteRenderer.gameObject.SetActive(false);
        SpriteRenderer.gameObject.SetActive(false);
        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        GameObject deathParts = Instantiate(DeathPartsPrefab, transform.position, Quaternion.identity);
        //deathParts.GetComponent<SortingLayer>().sod
    }

    private void InstantiateStates()
    {
        EmptyState = new PlayerEmptyState(this);
        IdleState = new PlayerIdleState(this);
        GroundedState = new PlayerGroundedState(this);
        FallState = new PlayerFallState(this);
        GroundPoundState = new PlayerGroundPoundState(this);
        RunState = new PlayerRunState(this);
        SlideState = new PlayerSlideState(this);
        JumpState = new PlayerJumpState(this);
        WallJumpState = new PlayerWallJumpState(this);
        WallSlideState = new PlayerWallSlideState(this);

    }

}
