using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{
    float slideEndTime;
    bool sliding = true;
    public PlayerSlideState(PlayerStateMachine psm, PlayerStateFactory psf) : base(psm, psf)
    {
        isRootState = true;
        //InitializeSubState();
    }

    public override void CheckSwitchStates()
    {
        if (context.IsTouchingWallRight || context.IsTouchingWallLeft)
        {
            context.CheckIfGrounded();
            if (!context.IsGrounded)
            {
                SwitchState(factory.Fall());
            }
            else
            {
                SwitchState(factory.Grounded());
            }
        }else if (!sliding)
        {
            if (!context.IsGrounded)
            {
                SwitchState(factory.Fall());
            }
            else
            {
                SwitchState(factory.Grounded());
            }
        }

        //throw new System.NotImplementedException();
    }

    public override void EnterState()
    {
        slideEndTime = Time.time + MovementStats.slideDuration;
        context.rb.velocity = new Vector2(MovementStats.slideSpeedX, MovementStats.fallSpeed/5);
    }

    public override void ExitState()
    {
        context.rb.velocity = new Vector2(0, 0);
    }

    public override void InitializeSubState()
    {
        SetSubState(factory.Empty());
    }

    public override void UpdateState()
    {
        if(Time.time > slideEndTime)
        {
            Debug.Log("NOT SLIDING");
            sliding = false;
        }

        CheckSwitchStates();
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
