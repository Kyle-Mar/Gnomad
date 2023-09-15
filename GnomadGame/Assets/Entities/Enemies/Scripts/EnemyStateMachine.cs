using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class EnemyStateMachine : StateMachine
{
    [Header("States")]

    //declare states here


    LayerMask groundLayerMask;
    ContactFilter2D floorContactFilter;
    ContactFilter2D wallContactFilter;

    [Header("Movement")]

    [SerializeField] bool isGrounded = false;

    [SerializeField] Vector2 lastMovementDirection = new(0, 0);

    [SerializeField] float currentMoveSpeed = MovementStats.moveSpeed;

    [Header("Components")]

    public Collider2D col;
    public Rigidbody2D rb;
    public SpriteRenderer SpriteRenderer;

    //public ParticleSystem WalkParticles;
    //public ParticleSystem JumpCloudParticles;
    //public ParticleSystem LandParticles;

    public bool IsGrounded => isGrounded;
    public Vector2 LastMovementDirection => lastMovementDirection;
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
        currentMoveSpeed = MovementStats.moveSpeed;

        //do instantiate AI = new EnemyAI();

        // this may need to be substituted with a sort of raycast+boxcast solution.
        floorContactFilter = new();
        floorContactFilter.SetLayerMask(groundLayerMask);
        floorContactFilter.SetNormalAngle(85, 95);


        InstantiateStates();

        //currentState = GroundedState;
        currentState.EnterState();
    }

    void Start()
    {
        isGrounded = ContextUtils.CheckIfGrounded(ref col, transform, ref floorContactFilter);
    }


    void Update()
    {
        isGrounded = ContextUtils.CheckIfGrounded(ref col, transform, ref floorContactFilter);
        currentState.UpdateStates();

    }


    public void SetMoveSpeed(float value)
    {
        currentMoveSpeed = value;
    }

    //this function will be totally changed when we figure out our AI model
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


    public void FlipComponents()
    {
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
    }

    private void InstantiateStates()
    {
        //instance all states here
    }
}
