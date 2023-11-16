using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerSlashingState : PlayerBaseState
{
    float slashEndTime;
    private float animatorInitialSpeed;
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
        //context.HatSpriteRenderer.enabled = false; //will be changed when animations are added
        slashEndTime = MovementStats.slashDuration;
        context.Animator.SetTrigger("SlashTrigger");
        context.SlashCollider.gameObject.SetActive(true);
        animatorInitialSpeed = context.Animator.speed;
        float animationLength = context.Animator.GetCurrentAnimatorStateInfo(0).length;
        context.Animator.speed = animationLength / MovementStats.slashDuration;
    }

    public override void ExitState()
    {
        context.IsSlashing = false;
        //context.HatSpriteRenderer.enabled = true; //will be changed when animations are added
        context.SlashCollider.gameObject.SetActive(false);
        context.UpdateComponentsDirection();
        context.Animator.speed = 1;
        Debug.Log("Done Slashing");

    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        //Debug.Log(currentSuperState);
        slashEndTime -= Time.deltaTime;
        Debug.Log("Slashing");
        CheckSwitchStates();
    }
}
