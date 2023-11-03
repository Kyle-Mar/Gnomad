using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorbChargeAttackState : EnemyBaseState
{
    public GorbChargeAttackState(EnemyStateMachine esm) : base(esm)
    {

    }
    
    private float attackTimer;


    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();

        if (attackTimer <= 0f)
        {
            SwitchState(context.NotAttackState);
        }

    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        context.SetMoveSpeed(context.ChargeSpeed);
        if (!context.animator.GetBool("InAir"))
        {
            context.animator.SetTrigger("ChargingTrigger");
        }
        context.animator.SetBool("Charging", true);
        context.IsAttacking = true;
        attackTimer = context.AttackDuration;
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
        context.SetMoveSpeed(context.MoveSpeed);
        context.StartAttackCooldown();
        //context.EnemyAttackObj.SetActive(false);
        context.animator.SetBool("Charging", false);
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
        //throw new System.NotImplementedException();
        attackTimer -= Time.deltaTime;
        CheckSwitchStates();
    }

    void Start()
    {

    }
}
