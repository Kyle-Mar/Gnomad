using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    float TimerChangeStateMax = 0.5f;
    float timerChangeState = 0f;

    public EnemyIdleState(EnemyStateMachine esm) : base(esm)
    {
    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();

        // If enemy is not aggro
        if (!context.IsAggro)
        {
            // Wait for timer to be done
            if (timerChangeState <= 0.0f)
            {
                int lastIndex = context.currentMovePointIndex;
                context.PickNextMovePoint();
                if (lastIndex != context.currentMovePointIndex || Vector2.Distance(context.transform.position, context.targetObject.transform.position) >= 0.4f)
                {
                    SwitchState(context.MoveState);
                }
            }
        }

        // If enemy is aggro
        else
        {
            if (Vector2.Distance(context.transform.position, context.targetObject.transform.position) > 0.5f)
            {
                if (context.IsAttackOnCooldown)
                {
                    if (timerChangeState <= 0.0f)
                    {
                        SwitchState(context.MoveState);
                    }
                }
                else
                {
                    SwitchState(context.MoveState);
                }
            }
        }
        
        
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        InitializeSubState();
        if (!context.animator.GetBool("InAir"))
        {
            context.animator.SetTrigger("IdleTrigger");
        }
        context.animator.SetFloat("Velocity", 0f);
        timerChangeState = TimerChangeStateMax;
        context.rb.Sleep();
        context.PickNextMovePoint();
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();

    }

    public override void FixedUpdateState()
    {
        // throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        //throw new System.NotImplementedException();
        SetSubState(context.NotAttackState);

    }

    public override void UpdateState()
    {
        //throw new System.NotImplementedException();

        CheckSwitchStates();

        //Debug.Log(context.targetObject.name);

        timerChangeState -= Time.deltaTime;
    }

    void Start()
    {

    }
}
