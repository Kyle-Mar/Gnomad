using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundPoundState : PlayerBaseState
{
    public PlayerGroundPoundState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        context.CheckIfGrounded();
        if (context.IsGrounded)
        {
            SwitchState(context.GroundedState);
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        context.ConsumeJumpBuffer();
    }

    public override void ExitState()
    {
        context.rb.velocity = new(0, 0);
        //throw new System.NotImplementedException();
    }

    public override void FixedUpdateState()
    {
       // throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        //throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        context.rb.velocity = Vector2.Lerp(context.rb.velocity,
                              new(MovementStats.groundPoundXSpeed, MovementStats.groundPoundYSpeed),
                              Utils.GetInterpolant(100f));
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
