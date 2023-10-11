using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;

public class EnemyStateMachine : StateMachine
{
    [Header("Stats")]
    //health, etc

    [Header("States")]

    //declare states here
    public EnemyEmptyState EmptyState;
    public EnemyGroundedState GroundedState;
    public EnemyMoveState MoveState;
    public EnemyIdleState IdleState;
    public EnemyAttackState AttackState;
    public EnemyNotAttackState NotAttackState;


    LayerMask groundLayerMask;
    ContactFilter2D floorContactFilter;

    [Header("Movement")]

    [SerializeField] bool isGrounded = false;

    [SerializeField] bool attackOnCooldown = false;

    [SerializeField] Vector2 lastMovementDirection = new(0, 0);

    [SerializeField] float currentMoveSpeed = MovementStats.moveSpeed;

    [SerializeField] bool isAttacking = false;

    [SerializeField] private const float ATTACK_COOLDOWN_MAX = 1f;

    [SerializeField] bool isTargetOutOfSight = false;

    // When the enemy can see the player, it becomes aggro
    // And when the player is far enough away or can't reach the player,
    //  it gets set back to false
    [SerializeField] bool isAggro = false;

    [SerializeField] private float attackCooldownTimer = 0f;

    [Header("Components")]

    public Collider2D col;
    public Rigidbody2D rb;
    public SpriteRenderer SpriteRenderer;
    public Animator animator;
    public Transform[] movePoints;
    public GameObject targetObject;
    public GameObject EnemyAttackObj;
    public int currentMovePointIndex;

    //public ParticleSystem WalkParticles;
    //public ParticleSystem JumpCloudParticles;
    //public ParticleSystem LandParticles;

    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }

    public bool IsAggro { get { return isAggro; } set { isAggro = value; } }

    public bool IsAttackOnCooldown { get { return attackOnCooldown; } set { attackOnCooldown = value; } }

    public bool IsTargetOutOfSight { get { return isTargetOutOfSight; } set { isTargetOutOfSight = value; } }

    public bool IsGrounded => isGrounded;

    //public Vector2 LastMovementDirection { get { return lastMovementDirection; } set { lastMovementDirection = value; } }
    public float CurrentMoveSpeed => currentMoveSpeed;



    private void OnEnable()
    {
        //enable AI
        
    }
    private void OnDisable()
    {
        //enable AI
    }

    private void Awake()
    {
        groundLayerMask = LayerMask.GetMask("Ground");
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = 5f;
        currentMovePointIndex = 0;

        //do instantiate AI = new EnemyAI();

        // Make sure there are at least two wandering points
        Assert.IsTrue(movePoints.Length >= 2);

        // Set One of the movePoints as the targetObject
        targetObject = movePoints[currentMovePointIndex].gameObject;

        // this may need to be substituted with a sort of raycast+boxcast solution.
        floorContactFilter = new();
        floorContactFilter.SetLayerMask(groundLayerMask);
        floorContactFilter.SetNormalAngle(85, 95);

        InstantiateStates();

        currentState = GroundedState;
        currentState.EnterState();
    }

    void Start()
    {
        isGrounded = ContextUtils.CheckIfGrounded(ref col, transform, ref floorContactFilter);
    }


    void Update()
    {
        isGrounded = ContextUtils.CheckIfGrounded(ref col, transform, ref floorContactFilter);
        UpdateMovementDirection();
        currentState.UpdateStates();
        if (IsAttackOnCooldown)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                IsAttackOnCooldown = false;
            }
        }
        
    }

    // Could pass in a parameter to change cooldown for specific attacks
    public void StartAttackCooldown()
    {
        IsAttacking = false;
        IsAttackOnCooldown = true;
        attackCooldownTimer = ATTACK_COOLDOWN_MAX;
    }

    public void CheckIfGrounded()
    {
        if (ContextUtils.CheckIfGrounded(ref col, transform, ref floorContactFilter))
        {
            isGrounded = true;
            animator.SetBool("InAir", false);
        }
        else
        {
            isGrounded = false;
            animator.SetBool("InAir", true);
        }
    }


    public void SetMoveSpeed(float value)
    {
        currentMoveSpeed = value;
    }

    //this function will be totally changed when we figure out our AI model
    private void UpdateMovementDirection()
    {
        // Probably use this for non aggro movement, and switch states based on its movement vector
        //  Instead of only using its Idle behavior as a timer
        
        animator.SetFloat("Velocity", Mathf.Abs(rb.velocity.x));


        if (rb.velocity.x <= -0.001 && SpriteRenderer.flipX)
        { FlipComponents(); }
        else if (rb.velocity.x >= 0.001 && !SpriteRenderer.flipX)
        { FlipComponents(); }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    
    public void FlipComponents()
    {
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
    }

    private void InstantiateStates()
    {
        //instance all states here
        EmptyState = new EnemyEmptyState(this);
        GroundedState = new EnemyGroundedState(this);
        MoveState = new EnemyMoveState(this);
        IdleState = new EnemyIdleState(this);
        AttackState = new EnemyAttackState(this);
        NotAttackState = new EnemyNotAttackState(this);
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && IsAttacking)
        {
            IDamageable damageable = null;
            if (collision.gameObject.TryGetComponent<IDamageable>(out damageable))
            {
                damageable.Damage(10f);
                Debug.Log("Damaging Player");
            }
        }
    }
}
