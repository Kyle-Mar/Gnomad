using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundedState : EnemyBaseState
{
    public EnemyGroundedState(EnemyStateMachine esm) : base(esm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        // Check for switch to jump state

        if (context.IsDamaged)
        {
            SwitchState(context.KnockbackState);
        }
        else if (!context.IsGrounded)
        {
            SwitchState(context.FallState);
        }
    }

    public override void EnterState()
    {
        context.animator.SetBool("InAir", false);
        InitializeSubState();
        //context.rb.velocity = new(context.rb.velocity.x, 0);
        //Object.Instantiate(context.LandParticles, context.Feet.position, Quaternion.identity);
    }

    public override void ExitState()
    {
        context.animator.SetTrigger("InAirTrigger");
        context.animator.SetBool("InAir", true);
        //throw new System.NotImplementedException();
    }
    
    public override void FixedUpdateState()
    {
        //throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        // Check the enemy movement vector to see if they are moving

        // Right now, it is just static behavior
        // Tells the enemy to move towards Move Point
        if (context.targetObject != null && Vector2.Distance(context.transform.position, context.targetObject.transform.position) >= 0.4f)
        {
            SetSubState(context.MoveState);
        }
        else
        {
            SetSubState(context.IdleState);
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
