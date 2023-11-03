using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for changing to an empty substate.
public class EnemyNotAttackState : EnemyBaseState
{
    public EnemyNotAttackState(EnemyStateMachine esm) : base(esm)
    {
    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();

        // If there is a targetObject
        if (context.targetObject)
        {
            // And the enemy is Aggro'd
            if (context.IsAggro)
            {
                // Get the distance between the two objects
                float dist = Vector2.Distance(context.gameObject.transform.position, context.targetObject.transform.position);
                //Debug.Log(dist);

                // If the enemy is close enough
                //  and the enemy's attack isn't on cooldown
                if (dist < 10f && !context.IsAttackOnCooldown && !context.IsTargetOutOfSight)
                {
                    // Switch to Attack State if it isn't already
                    if (currentSubState != context.AttackState)
                    {
                        SwitchState(context.AttackState);
                    }
                }
            }
            
        }
        
        
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        context.IsAttacking = false;
        context.JustAttacked = true;
        context.animator.SetBool("Charging", false);
        CheckSwitchStates();
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
    }

    void Start()
    {

    }
}
