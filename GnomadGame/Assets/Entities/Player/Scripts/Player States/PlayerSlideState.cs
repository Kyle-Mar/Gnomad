using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerSlideState : PlayerBaseState
{
    Vector2 initialMovementDir;
    float slideEndTime;
    bool sliding = true;
    float verticalBounceEndTime;
    public float TryingToTurn;
    float timeSinceStart = 0f;

    public PlayerSlideState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        if (context.CanJumpStandard())
        {
            context.ConsumeJumpBuffer();
            SwitchState(context.BackflipState);
        }
        if (DidBonk())
        {
            if (!context.IsGrounded)
            {
                SwitchState(context.FallState);
            }
            else
            {
                SwitchState(context.GroundedState);
            }
        }
        else if (!sliding)
        {
            if (!context.IsGrounded)
            {
                SwitchState(context.FallState);
            }
            else
            {
                SwitchState(context.GroundedState);
            }
        }
    }


    public override void EnterState()
    {
        InitializeSubState();
        //context.SpriteRenderer.transform.rotation = Quaternion.Euler(0,0,90*context.LastMovementDirection.x*-1); //will be changed when animations are added
        //context.HatSpriteRenderer.enabled = false; //will be changed when animations are added
        initialMovementDir = context.LastMovementDirection;
        sliding = true;

        //context.SlideCollider.gameObject.SetActive(true);

        slideEndTime = Time.time + MovementStats.slideDuration;
        verticalBounceEndTime = Time.time + MovementStats.slideVerticalBounceDuration;
        context.rb.velocity = new Vector2(MovementStats.slideSpeedX * initialMovementDir.x, MovementStats.slideVerticalBounce);
        context.Animator.SetTrigger("SlideTrigger");
        timeSinceStart = 0f;
    }

    public override void ExitState()
    {
        //context.SpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0); //will be changed when animations are added
        context.SlideCollider.gameObject.SetActive(false);
        //context.HatSpriteRenderer.enabled = true; //will be changed when animations are added
        context.rb.velocity = new Vector2(0, 0);
        context.DoSlideCooldownTimer();
        context.UpdateComponentsDirection();
        context.Animator.SetTrigger("StopSlideTrigger");

    }
    public override void FixedUpdateState()
    {
        //Debug.Log("Time.Time: " + Time.time + " slideEndTime " + slideEndTime);
        if (Time.time > slideEndTime)
        {
            sliding = false;
        }
        else if (Time.time > verticalBounceEndTime)
        {
            context.rb.velocity = new Vector2(MovementStats.slideSpeedX * initialMovementDir.x, MovementStats.slideFallSpeed);
        }
        else
        {
            Debug.Log("SlideBounceStillActive");
            context.rb.velocity = new Vector2(MovementStats.slideSpeedX * initialMovementDir.x, MovementStats.slideVerticalBounce);
        }
    }

    public override void InitializeSubState()
    {
        SetSubState(context.EmptyState);
    }

    public override void UpdateState()
    {
        timeSinceStart += Time.deltaTime;
        if (!context.SlideCollider.gameObject.activeInHierarchy && timeSinceStart >= 0.05f)
        {
            context.SlideCollider.gameObject.SetActive(true);
            Debug.LogWarning("Set To True");
        }
        CheckSwitchStates();
    }

    bool DidBonk()
    {
        return (context.IsTouchingWallRight && initialMovementDir.x > 0
            || context.IsTouchingWallLeft && initialMovementDir.x < 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
