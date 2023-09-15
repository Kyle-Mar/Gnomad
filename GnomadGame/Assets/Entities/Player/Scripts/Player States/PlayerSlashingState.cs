using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayerSlashingState : PlayerBaseState
{
    Vector2 slashDirection;
    float slashEndTime;
    public PlayerSlashingState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void CheckSwitchStates()
    {
        if (slashEndTime <= 0)
        {
            SwitchState(context.NotAttackState);
        }
    }

    public override void EnterState()
    {
        context.IsSlashing = true;
        slashEndTime = MovementStats.slashDuration;
        slashDirection = context.LastMovementDirection;

        context.SlashCollider.transform.localScale = new Vector3(slashDirection.x, 1, 1);
        context.SlashCollider.enabled = true;
    }

    public override void ExitState()
    {
        context.IsSlashing = false;
        context.SlashCollider.enabled = false;
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        slashEndTime -= Time.deltaTime; 
        CheckSwitchStates();
    }
}
