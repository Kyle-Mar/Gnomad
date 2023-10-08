using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    float TimerChangeStateMax = 1f;
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
                // Move to the next move point
                if (context.currentMovePointIndex >= context.movePoints.Length - 1)
                {
                    context.currentMovePointIndex = 0;
                }
                else
                {
                    context.currentMovePointIndex++;
                }
                context.targetObject = context.movePoints[context.currentMovePointIndex].gameObject;
                SwitchState(context.MoveState);
            }
        }

        // If enemy is aggro
        else
        {
            float dist = Vector2.Distance(context.gameObject.transform.position, context.targetObject.transform.position);
            Debug.Log(dist);
            if ( dist < 1.7f)
            {
                SetSubState(context.AttackState);
            }
            else
            {
                SetSubState(context.NotAttackState);
            }
        }
        
        
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        InitializeSubState();
        timerChangeState = TimerChangeStateMax;
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
        
        

        timerChangeState -= Time.deltaTime;
    }

    void Start()
    {

    }
}
