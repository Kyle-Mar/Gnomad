using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{

    PlayerBaseState currentState;
    PlayerStateFactory states;
    public PlayerControls Controls;

    LayerMask groundLayerMask;
    ContactFilter2D floorContactFilter;
    ContactFilter2D wallContactFilter;

    [SerializeField] bool isGrounded = false;
    [SerializeField] bool isTouchingWallLeft = false;
    [SerializeField] bool isTouchingWallRight = false;

    [SerializeField] Vector2 lastMovementDirection = new(0,0);

    [SerializeField] float jumpBufferTime = 0f;
    [SerializeField] float currentMoveSpeed = MovementStats.moveSpeed;

    public Collider2D col;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer hatSpriteRenderer;
    public PhysicsMaterial2D sticky;
    public PhysicsMaterial2D slippery;
    public GameObject deathPartsPrefab;
    public Camera cam;
    public Transform feet;

    [Header("Zone Dependent Assets")]
    public ParticleSystem walk_particles;
    public ParticleSystem jump_cloud_particles;
    public ParticleSystem land_particles;


    public PlayerBaseState CurrentState { get { return currentState; } set { currentState = value; } }
    public bool IsGrounded { get { return isGrounded; } private set { } }
    public float JumpBufferTime { get { return jumpBufferTime; } private set { } }
    public bool IsTouchingWallLeft { get { return isTouchingWallLeft; } private set { } }
    public bool IsTouchingWallRight { get { return isTouchingWallRight; } private set { } }
    public bool IsTouchingWall { get { return IsTouchingWallLeft || IsTouchingWallRight; } private set { } }
    public Vector2 LastMovementDirection { get { return lastMovementDirection; } private set { } }

    public float CurrentMoveSpeed { get { return currentMoveSpeed; } private set { } }


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
        cam = GetComponent<Camera>();

        //spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //hatSpriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();

        Assert.IsNotNull(spriteRenderer);
        Assert.IsNotNull(hatSpriteRenderer);
        Assert.IsNotNull(deathPartsPrefab);
        Assert.IsNotNull(feet);


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

    private void FixedUpdate()
    {
        currentState.FixedUpdateStates();
    }

    void Update()
    {
        //Debug.Log("TEST");
        CheckIfGrounded();
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

    public void SetMoveSpeed(float value)
    {
        currentMoveSpeed = value;
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
        Debug.Log(IsTouchingWallLeft);
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
        spriteRenderer.flipX = !spriteRenderer.flipX;
        hatSpriteRenderer.flipX = !hatSpriteRenderer.flipX;
        if (!spriteRenderer.flipX)
        {
        //flip hinge joint angles
        }
    }

    public void Die(InputAction.CallbackContext cxt)
    {//this is a temperary function mostly for shits &/Or giggles
     //this should probably be it's own state and this code is not 
     //necessarily great
        hatSpriteRenderer.gameObject.SetActive(false);
        spriteRenderer.gameObject.SetActive(false);
        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        GameObject deathParts = Instantiate(deathPartsPrefab, transform.position, Quaternion.identity);
        //deathParts.GetComponent<SortingLayer>().sod
    }
}
