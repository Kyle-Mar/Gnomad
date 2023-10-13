using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorbChargeAttackState : EnemyBaseState
{
    public GorbChargeAttackState(EnemyStateMachine esm) : base(esm)
    {

    }
    
    private float attackTimer;
    private const float ATTACK_TIMER_MAX = 1.5f;

    const float CHARGING_MOVEMENT_SPEED = 12.5f;
    const float WALKING_MOVEMENT_SPEED = 5f;



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
        context.SetMoveSpeed(CHARGING_MOVEMENT_SPEED);
        context.animator.SetBool("Charging", true);
        context.IsAttacking = true;
        attackTimer = ATTACK_TIMER_MAX;
        Debug.Log("Enemy Attacking");
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
        context.SetMoveSpeed(WALKING_MOVEMENT_SPEED);
        context.StartAttackCooldown();
        context.EnemyAttackObj.SetActive(false);
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
