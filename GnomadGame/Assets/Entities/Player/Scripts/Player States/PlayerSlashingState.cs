using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayerSlashingState : PlayerBaseState
{
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
        //Debug.Log("SLASHING ENTER");
        context.HatSpriteRenderer.enabled = false; //will be changed when animations are added
        slashEndTime = MovementStats.slashDuration;
        context.Animator.SetTrigger("SlashTrigger");
        context.SlashCollider.gameObject.SetActive(true);
    }

    public override void ExitState()
    {
        context.IsSlashing = false;
        context.HatSpriteRenderer.enabled = true; //will be changed when animations are added
        context.SlashCollider.gameObject.SetActive(false);
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
