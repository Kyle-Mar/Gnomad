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
        if (timerChangeState <= 0.0f)
        {
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

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
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
