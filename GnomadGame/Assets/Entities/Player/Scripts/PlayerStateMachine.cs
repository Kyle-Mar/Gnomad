using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerStateMachine : StateMachine
{
    [Header("States")]
    #region properties and fields
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
    public PlayerSlashingState SlashState;
    public PlayerNotAttackingState NotAttackState;
    public PlayerDeathState DeathState;
    public PlayerGPBounceState GPBounceState;
    public PlayerDashState DashState;
    public PlayerBackflipState BackflipState;
    public PlayerKnockbackState KnockbackState;
    public PlayerControls Controls;

    LayerMask groundLayerMask;
    ContactFilter2D floorContactFilter;
    ContactFilter2D wallContactFilter;

    [Header("Movement")]

    [SerializeField] bool isGrounded = false;
    [SerializeField] bool isOnCeiling = false;
    [SerializeField] bool isTouchingWallLeft = false;
    [SerializeField] bool isTouchingWallRight = false;
    [SerializeField] bool wallSlideExpired = false;
    [SerializeField] bool isSlashing = false;
    [SerializeField] bool isGPBounceQueued = false;
    [SerializeField] bool canDash = true;
    [SerializeField] bool canSlide = true;
    [SerializeField] bool inCoyoteRange = false;
    [SerializeField] Vector2 lastMovementDirection;
    [SerializeField] float jumpBufferTime = 0f;
    [SerializeField] float leftInputBufferTime = 0f;
    [SerializeField] float rightInputBufferTime = 0f;
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
    public Collider2D SlashCollider;
    public Collider2D GroundPoundCollider;
    public Collider2D SlideCollider;
    public Animator Animator;
    public Collider2D HurtBox;
    [Header("Zone Dependent Assets")]
    public ParticleSystem WalkParticles;
    public ParticleSystem JumpCloudParticles;
    public ParticleSystem LandParticles;
    public ParticleSystem[] HurtParticles;


    public bool IsGrounded => isGrounded;
    public bool IsOnCeiling => isOnCeiling;
    public float JumpBufferTime => jumpBufferTime;
    public bool IsTouchingWallLeft => isTouchingWallLeft;
    public bool IsTouchingWallRight => isTouchingWallRight;
    public bool IsGPBounceQueued { get { return isGPBounceQueued; } set { isGPBounceQueued = value; } }
    public bool CanDash { get { return canDash; } set { canDash = value; } }
    public bool CanSlide { get { return canSlide; } set { canSlide = value; } }
    public bool IsSlashing { get { return isSlashing; } set { isSlashing = value; } }
    public bool InCoyoteRange { get { return inCoyoteRange; } set { inCoyoteRange = value; } }

    public bool IsTouchingWall => isTouchingWallLeft || isTouchingWallRight;
    public bool WallSlideExpired => wallSlideExpired;
    public Vector2 LastMovementDirection { get { return lastMovementDirection; } set { } }
    public float CurrentMoveSpeed => currentMoveSpeed;

    public Vector3 LastKBDirection = Vector3.zero;
    public delegate void OnDamageKB(float amt, Collider2D collider, Vector3 dir);
    public OnDamageKB onDamageKB;
    #endregion

    private void OnEnable()
    {
        Controls.Player.Move.performed += UpdateMovementDirection;
        Controls.Player.Die.performed += Die;
        Controls.Player.PrintStateTree.performed += PrintDebugInfo;
        Controls.Player.Respawn.performed += ReloadScene;
        GetComponentInChildren<Health>().onDeath += DieHealth;
        GetComponentInChildren<Health>().onDamage += OnDamage;
        Controls.Enable();
    }
    private void OnDisable()
    {
        GetComponentInChildren<Health>().onDeath += DieHealth;
        Controls.Disable();
    }

    private void Awake()
    {
        groundLayerMask = LayerMask.GetMask("Ground");
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = MovementStats.moveSpeed;
        Cam = GetComponent<Camera>();


        Assert.IsNotNull(SpriteRenderer);
        Assert.IsNotNull(HatSpriteRenderer);
        Assert.IsNotNull(DeathPartsPrefab);
        Assert.IsNotNull(Feet);
        Assert.IsNotNull(SlashCollider);
        Assert.IsNotNull(GroundPoundCollider);
        Assert.IsNotNull(SlideCollider);
        Assert.IsNotNull(HurtBox);

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
        lastMovementDirection = new Vector2(1, 0);
    }
    void Start()
    {
    }

    void Update()
    {
        //Debug.Log(isGrounded);
        //InputAction.CallbackContext callbackContext = new();
        //PrintDebugInfo(callbackContext);
        DoJumpBuffer();
        DoInputBuffer();
        currentState.UpdateStates();
        currentState.PrintStateTree();

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

    private void DoInputBuffer()
    {
        if(Controls.Player.Move.ReadValue<Vector2>().x > 0)
        {
            rightInputBufferTime = .15f;
        }
        else if(rightInputBufferTime >= 0f)
        {
            rightInputBufferTime -= Time.deltaTime;
        }
        if(Controls.Player.Move.ReadValue<Vector2>().x < 0)
        {
            leftInputBufferTime = .15f;
        }
        else if(leftInputBufferTime >= 0)
        {
            leftInputBufferTime -= Time.deltaTime;
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
        //Debug.Log(inputVector);
        if (inputVector.x < -0.15)
        {
            lastMovementDirection.x = -1;
        }
        else if (inputVector.x > 0.15)
        {
            lastMovementDirection.x = 1;
        }
        //all following code only happens if we turn the sprites
        if (currentState == SlashState || currentState == DashState || currentState == SlideState) { return; }

        UpdateComponentsDirection();
    }

    public void ConsumeLeftInputBuffer()
    {
        leftInputBufferTime = 0f;
    }
    public void ConsumeRightInputBuffer()
    {
        rightInputBufferTime = 0f;
    }

    public (Vector2, bool) DoWallSlide()
    {
        if (IsGrounded)
        {
            return (Vector2.zero, false);
        }

        if (wallSlideExpired)
        {
            return (Vector2.zero, false);
        }

        float inputX = Controls.Player.Move.ReadValue<Vector2>().x;
        if (IsTouchingWallLeft && inputX < 0 || (IsTouchingWallLeft && leftInputBufferTime > 0f))
        {
            ConsumeLeftInputBuffer();
            return (Vector2.left, true);
        }
        if (IsTouchingWallRight && inputX > 0 || (IsTouchingWallRight && leftInputBufferTime > 0f))
        {
            ConsumeRightInputBuffer();
            return (Vector2.right, true);
        }
        return (Vector2.zero, false);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ContextUtils.CheckIfGrounded(collision))
        {
            isGrounded = true;
            wallSlideExpired = false;
            canDash = true;
        }
        if(ContextUtils.CheckIfOnCeiling(collision, col))
        {
            isOnCeiling = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            if (ContextUtils.CheckIfGrounded(collision))
            {
                isGrounded = true;
                wallSlideExpired = false;
                canDash = true;
            }
            else
            {
                if (isGrounded)
                {
                    StartCoroutine(StartCoyoteTimer());
                }
                isGrounded = false;
            }
            if(ContextUtils.CheckIfOnCeiling(collision, col))
            {
                isOnCeiling = true;
            }
            else
            {
                isOnCeiling = false;
            }

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
        if (isTouchingWallLeft || isTouchingWallRight) { CanDash = true; }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.transform.tag == "Ground")
        {
            isTouchingWallLeft = false;
            isTouchingWallRight = false;
        }
        if (!ContextUtils.CheckIfGrounded(collision, col))
        {
            if (isGrounded)
            {
                StartCoroutine(StartCoyoteTimer());
            }
            isGrounded = false;
        }

        if(!ContextUtils.CheckIfOnCeiling(collision, col))
        {
            isOnCeiling = false;
        }
    }

    public void UpdateComponentsDirection(bool flipDirection=false)
    {
        Vector2 sliderHitboxOffset = SlideCollider.GetComponent<BoxCollider2D>().offset;
        Vector2 slashHitboxOffset = SlashCollider.GetComponent<PolygonCollider2D>().offset;

        //SpriteRenderer.flipX = !SpriteRenderer.flipX;
        float facingDirection = LastMovementDirection.x;
        Vector3 tmp = SpriteRenderer.gameObject.transform.localScale;
        if (flipDirection) { facingDirection *= -1; }

        transform.localScale = new Vector2(facingDirection, tmp.y);
        //HatSpriteRenderer.flipX = !HatSpriteRenderer.flipX;
        //SlideCollider.GetComponent<BoxCollider2D>().offset = new Vector2(-1 * sliderHitboxOffset.x, sliderHitboxOffset.y);
        //SlideCollider.transform.localScale = new Vector2(SlideCollider.transform.localScale.x*-1, SlideCollider.transform.localScale.y);
        //SlashCollider.GetComponent<PolygonCollider2D>().offset = new Vector2(-1 * slashHitboxOffset.x, slashHitboxOffset.y);
        //SlashCollider.transform.localScale = new Vector2(SlashCollider.transform.localScale.x * -1, SlashCollider.transform.localScale.y);


    }

    public void Die(InputAction.CallbackContext cxt)
    {//this is a temperary function mostly for shits &/Or giggles
     //this should probably be it's own state and this code is not 
     //necessarily great

        //deathParts.GetComponent<SortingLayer>().sod

        
        if (currentState != DeathState)
        {
            currentState = DeathState;
            currentState.EnterState();
        }
    }

    public void ReloadScene(InputAction.CallbackContext cxt)
    {
        // Load Scene Stuff Here
        //StartCoroutine(LevelManager.LoadSceneAsync("reloadScene"));
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
        SlashState = new PlayerSlashingState(this);
        NotAttackState = new PlayerNotAttackingState(this);
        DeathState = new PlayerDeathState(this);
        GPBounceState = new PlayerGPBounceState(this);
        DashState = new PlayerDashState(this);
        BackflipState = new PlayerBackflipState(this);
        KnockbackState = new PlayerKnockbackState(this);
    }

    void DieHealth()
    {
        if (CurrentState != DeathState)
        {
            CurrentState = DeathState;
            CurrentState.EnterState();
        }
    }

    void OnDamage(float amount, Collider2D collider, Vector3 dir)
    {
        foreach (ParticleSystem ps in HurtParticles)
        {
            if (ps != null)
            {
                Instantiate(ps, transform.position, Quaternion.identity);
            }
        }
        LastKBDirection = dir;
        onDamageKB?.Invoke(amount, collider, dir);
    }

    void PrintDebugInfo(InputAction.CallbackContext cxt)
    {
        CurrentState.PrintStateTree();
        //Debug.Log(currentMoveSpeed);
        //GetComponentInChildren<Health>().Damage(2);
        //Debug.Log("Screen Shot Take");
        ScreenShot();
    }

    void ScreenShot()
    {
        ScreenCapture.CaptureScreenshot("SuperSpecialScreenShot.png", 1);
    }

    public void DoDashCooldownTimer()
    {
        StartCoroutine(StartDashCooldownTimer());
    }
    public IEnumerator StartDashCooldownTimer()
    {
        canDash = false;
        yield return new WaitForSeconds(MovementStats.DashCooldown);
        CanDash = true;
    }

    public IEnumerator StartCoyoteTimer()
    {
        InCoyoteRange = true;
        yield return new WaitForSeconds(MovementStats.CoyoteTime);
        InCoyoteRange = false;
    }

    public void DoSlideCooldownTimer()
    {
        StartCoroutine(StartSlideTimer());
    }

    public IEnumerator StartSlideTimer()
    {
        CanSlide = false;
        yield return new WaitForSeconds(MovementStats.slideCooldowntimer);
        CanSlide = true;
    }

    public bool CanJumpStandard()
    {
        return (((JumpBufferTime > 0) ||  Controls.Player.Jump.WasPressedThisFrame()) && IsGrounded) || (Controls.Player.Jump.WasPressedThisFrame() && InCoyoteRange);
    }

    public bool CheckCanSlash()
    {
        return (currentState != DashState && currentState != SlideState);
    }
}
