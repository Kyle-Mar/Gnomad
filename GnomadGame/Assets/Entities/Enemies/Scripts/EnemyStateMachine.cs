using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;
using System;

public class EnemyStateMachine : StateMachine
{
    [SerializeField] public EnemyMovementStats Stats;
    #region properties and fields
    [Header("Stats")]
    //health, etc

    [Header("States")]

    //declare states here
    public EnemyEmptyState EmptyState;
    public BaseState GroundedState;
    public BaseState FallState;
    public BaseState KnockbackState;

    public BaseState MoveState;
    public BaseState IdleState;
    
    public BaseState AttackState;
    public BaseState NotAttackState;


    LayerMask groundLayerMask;
    ContactFilter2D floorContactFilter;



    [Header("Movement")]

    // For knockback:
        // 1 = right
        // -1 = left
    public int damageDirection = 1;

    [SerializeField] bool isDamaged = false;

    [SerializeField] bool isGrounded = false;
    [SerializeField] bool isOnCeiling = false;

    [SerializeField] bool attackOnCooldown = false;

    [SerializeField] float currentMoveSpeed = 10f;

    [SerializeField] bool isAttacking = false;

    [SerializeField] bool isTargetOutOfSight = false;

    // When the enemy can see the player, it becomes aggro
    // And when the player is far enough away or can't reach the player,
    //  it gets set back to false
    [SerializeField] bool isAggro = false;

    [SerializeField] bool justAttacked = false;

    [SerializeField] private float attackCooldownTimer = 0f;

    [SerializeField] public bool isSlidedInto = false;

    [SerializeField] bool isVolleyed = false;

    [SerializeField] bool isVolleyedInto = false;

    [Header("Components")]

    public Collider2D col;
    public Rigidbody2D rb;
    public SpriteRenderer SpriteRenderer;
    public Animator animator;
    public List<Transform> movePoints = new();
    public GameObject targetObject;
    //public GameObject EnemyAttackObj;
    public int currentMovePointIndex;
    public EnemyMovementStats EnemyStats;
    public ParticleSystem[] OnHitParticles;
    public ParticleSystem[] OnDeathParticles;
    public Collider2D volleyCol;
    public Knockable knockableComp;

    //public ParticleSystem WalkParticles;
    //public ParticleSystem JumpCloudParticles;
    //public ParticleSystem LandParticles;

    public bool IsVolleyed { get { return isVolleyed; } set { isVolleyed = value; } }

    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }

    public bool IsAggro { get { return isAggro; } set { isAggro = value; } }

    public bool IsAttackOnCooldown { get { return attackOnCooldown; } set { attackOnCooldown = value; } }

    public bool IsTargetOutOfSight { get { return isTargetOutOfSight; } set { isTargetOutOfSight = value; } }

    public bool JustAttacked { get { return justAttacked; } set { justAttacked = value; } }

    public bool IsGrounded => isGrounded;

    public bool IsOnCeiling => isOnCeiling;

    public bool IsDamaged { get { return isDamaged; } set { isDamaged = value; } }

    public bool IsSlidedInto { get { return isSlidedInto; } set { isSlidedInto = value; } }

    public bool IsVolleyedInto { get { return isVolleyedInto; } set { isVolleyedInto = value; } }

    //public Vector2 LastMovementDirection { get { return lastMovementDirection; } set { lastMovementDirection = value; } }
    public float CurrentMoveSpeed => currentMoveSpeed;

    public float MoveSpeed => GorbStats.moveSpeed;
    public float MoveSpeedReduced => GorbStats.moveSpeedReduced;
    public float ChargeSpeed => GorbStats.chargeSpeed;
    public float JumpMoveSpeed => GorbStats.jumpMoveSpeed;
    public float FallSpeed => GorbStats.fallSpeed;
    public float MaxJumpHeight => GorbStats.maxJumpHeight;
    public float AttackDamage => GorbStats.attackDamage;
    public float AttackCooldown => GorbStats.attackCooldown;
    public float AttackDuration => GorbStats.attackDuration;

    public float KnockbackXConst => GorbStats.knockbackXConst;
    public float KnockbackYConst => GorbStats.knockbackYConst;
    public float KnockbackTimer => GorbStats.knockbackTimer;

    public float knockbackSpeed = 12.5f;
    // Connected to EnemyBaseState's OnKB function
    public delegate void OnKnockback();
    public OnKnockback onKB;
    #endregion
    private void OnEnable()
    {
        //enable AI
        GetComponentInChildren<Health>().onDeath += DieHealth;
        GetComponentInChildren<Health>().onDamage += OnDamage;
        GetComponentInChildren<Knockable>().onKnockback += ApplyKnockback;

    }
    private void OnDisable()
    {
        //enable AI
        GetComponentInChildren<Health>().onDeath += DieHealth;
    }

    private void Awake()
    {
        groundLayerMask = LayerMask.GetMask("Ground");
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        knockableComp = GetComponent<Knockable>();
        //EnemyAttackObj = null;

        currentMoveSpeed = GorbStats.moveSpeed;
        currentMovePointIndex = 0;

        //do instantiate AI = new EnemyAI();

        // Make sure there are at least two wandering points

        if (movePoints.Count <= 0)
        {
            // If there aren't create them at a good common offset
            var mp = new GameObject("MovePoint0");
            mp.transform.parent = gameObject.transform.parent;
            mp.transform.position = new(transform.position.x + 5, transform.position.y, transform.position.z);
            var mp1 = new GameObject("MovePoint1");
            mp1.transform.parent = gameObject.transform.parent;
            mp1.transform.position = new(transform.position.x - 5, transform.position.y, transform.position.z);
            movePoints.Add(mp.transform);
            movePoints.Add(mp1.transform);

        }

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
    }


    void Update()
    {
        UpdateMovementDirection();
        currentState.UpdateStates();
        //Debug.LogWarning(currentState.ToString());
        //Debug.LogWarning(currentState.CurrentSubState().ToString());
        if (IsAttackOnCooldown)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                IsAttackOnCooldown = false;
            }
        }
        
    }

    public void ApplyKnockback()
    {
        // Switches state to Knockback State inside EnemyBaseState
        onKB?.Invoke();
    }


    // Builder Functions

    public EnemyStateMachine BuildAttackState(BaseState newState)
    {
        if (newState == null)
        {
            AttackState = new EnemyAttackState(this);
        }
        else
        {
            AttackState = newState;
        }

        return this;
    }

    public EnemyStateMachine BuildMoveState(BaseState newState)
    {
        if (newState == null)
        {
            MoveState = new EnemyMoveState(this);
        }
        else
        {
            MoveState = newState;
        }

        return this;
    }

    public EnemyStateMachine BuildIdleState(BaseState newState)
    {
        if (newState == null)
        {
            IdleState = new EnemyIdleState(this);
        }
        else
        {
            IdleState = newState;
        }

        return this;
    }

    public EnemyStateMachine BuildNotAttackState(BaseState newState)
    {
        if (newState == null)
        {
            NotAttackState = new EnemyNotAttackState(this);
        }
        else
        {
            NotAttackState = newState;
        }

        return this;
    }

    public EnemyStateMachine BuildGroundedState(BaseState newState = null)
    {
        if (newState == null)
        {
            GroundedState = new EnemyGroundedState(this);
        }
        else
        {
            GroundedState = newState;
        }

        return this;
    }

    public EnemyStateMachine BuildFallState(BaseState newState = null)
    {
        if (newState == null)
        {
            FallState = new EnemyFallState(this);
        }
        else
        {
            FallState = newState;   
        }

        return this;
    }


    // Could pass in a parameter to change cooldown for specific attacks
    public void StartAttackCooldown()
    {
        IsAttacking = false;
        IsAttackOnCooldown = true;
        attackCooldownTimer = AttackCooldown;
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
        
        if (rb.velocity.x <= -0.001 && SpriteRenderer.flipX && !IsDamaged)
        { FlipComponents(); }
        else if (rb.velocity.x >= 0.001 && !SpriteRenderer.flipX && !IsDamaged)
        { FlipComponents(); }
    }

    
    public void FlipComponents()
    {
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
    }

    private void InstantiateStates()
    {
        //instance all states here
        if (EmptyState == null)
        {
            EmptyState = new EnemyEmptyState(this);
        }
        if (GroundedState == null)
        {
            GroundedState = new EnemyGroundedState(this);
        }
        if (MoveState == null)
        {
            MoveState = new EnemyMoveState(this);
        }
        if (IdleState == null)
        {
            IdleState = new EnemyIdleState(this);
        }
        if (AttackState == null)
        {
            AttackState = new GorbChargeAttackState(this);
        }
        if (NotAttackState == null)
        {
            NotAttackState = new EnemyNotAttackState(this);
        }
        if (KnockbackState == null)
        {
            KnockbackState = new EnemyKnockbackState(this);
        }
    }

    void DieHealth()
    {
        foreach (ParticleSystem ps in OnDeathParticles)
        {
            if (ps != null)
            {//nullify the parent so the particles persist after death
                Instantiate(ps, transform.position, Quaternion.identity).transform.parent = null;

            }
        }
        Destroy(transform.parent.gameObject);
    }


    // Probably want to remove dir as a parameter for OnDamage
    // dir should probably be handled by IKnockable
    // I agree - Kyle

    void OnDamage(float amount, Collider2D collider, Vector3 dir)
    {
        Debug.Log(collider +" "+ dir);
        if (currentState != KnockbackState)
        {
            IsDamaged = true;
            foreach (ParticleSystem ps in OnHitParticles)
            {
                if (ps != null)
                {
                    Instantiate(ps, transform.position, Quaternion.identity);
                }
            }
        }
        if (!IsGrounded)
        {
            //IsSlidedInto = false;
            IsVolleyed = true;
            CameraSystemEvent.onShake?.Invoke();
            volleyCol.gameObject.SetActive(true);
            //Debug.LogWarning("Enemy Volleyed");
        }
        else
        {
            IsVolleyed = false;   
        }
        //LastKBDirection = dir;
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("EnteringTrigger");
        if (collision.CompareTag("Enemy"))
        {
            if (collision.GetComponentInParent<EnemyStateMachine>().IsVolleyed && IsVolleyed) { return; }//prevent volleyed enemies from cancelling each other out
            IDamageable damageable;
            IKnockable knockable;
            if (collision.gameObject.TryGetComponent(out damageable))
            {
                var collisionPoint = collision.ClosestPoint(transform.position);
                var dir = collisionPoint - new Vector2(transform.position.x, transform.position.y);
                collision.GetComponentInParent<EnemyStateMachine>().IsVolleyedInto = true;
                if (collision.gameObject.TryGetComponent(out knockable))
                {
                    knockable.Knockback(dir);
                }
                damageable.Damage(AttackDamage, volleyCol, dir);
            }
        }
        else if (collision.CompareTag("Ground") && IsVolleyed)
        {
            if (currentState != FallState)
            {
                currentState.ExitState();
                currentState = FallState;
            }
        }

    }

    public void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("PlayerHurtBox"))
        {
            IDamageable damageable;
            if (collision.gameObject.TryGetComponent(out damageable))
            {
                var collisionPoint = collision.ClosestPoint(transform.position);
                var dur = collisionPoint - new Vector2(transform.position.x, transform.position.y);
                damageable.Damage(AttackDamage, col, dur);
            }
        }
    }

    public void PickNextMovePoint()
    {
        // Move to the next move point
        if (currentMovePointIndex >= movePoints.Count - 1)
        {
            currentMovePointIndex = 0;
        }
        else
        {
            currentMovePointIndex++;
        }
        targetObject = movePoints[currentMovePointIndex].gameObject;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        

        if(collision.gameObject.tag == "Ground")
        {
            if (ContextUtils.CheckIfGrounded(collision))
            {
                isGrounded = true;
                animator.SetBool("InAir", false);
                IsVolleyed = false;
            }
            else
            {
                isGrounded = false;
                if (!animator.GetBool("InAir"))
                {
                    animator.SetTrigger("InAirTrigger");
                    animator.SetBool("InAir", true);
                }
            }
        }
        if (ContextUtils.CheckIfOnCeiling(collision, col))
        {
            isOnCeiling = true;
        }
        else
        {
            isOnCeiling = false;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(ContextUtils.CheckIfOnCeiling(collision, col))
        {
            isOnCeiling = true;
        }

        /*
        if(collision.gameObject.tag == "Enemy")
        {
            PickNextMovePoint();
        }
        */
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            if (!ContextUtils.CheckIfGrounded(collision))
            {
                isGrounded = false;
                
                animator.SetTrigger("InAirTrigger");
                animator.SetBool("InAir", true);
            }
        }
        if(!ContextUtils.CheckIfOnCeiling(collision, col))
        {
            isOnCeiling = false;
        }
    }
}
