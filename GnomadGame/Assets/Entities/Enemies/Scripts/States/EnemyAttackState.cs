using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{

    [SerializeField] private float attackTimer;

    public EnemyAttackState(EnemyStateMachine esm) : base(esm)
    {
    }

    

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();


        if (attackTimer <= 0f)
        {
            context.IsAttacking = false;
            SwitchState(context.NotAttackState);
        }
        else if (context.IsDamaged)
        {
            context.IsAttacking = false;
            SwitchState(context.NotAttackState);
        }

    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();

        //context.EnemyAttackObj.SetActive(true);
        context.IsAttacking = true;
        attackTimer = context.AttackDuration;
        Debug.Log("Enemy Attacking");
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
        context.StartAttackCooldown();
        //context.EnemyAttackObj.SetActive(false);
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
