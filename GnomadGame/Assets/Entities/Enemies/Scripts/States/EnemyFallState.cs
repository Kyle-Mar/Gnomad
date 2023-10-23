using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFallState : EnemyBaseState
{
    public EnemyFallState(EnemyStateMachine esm) : base(esm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        context.CheckIfGrounded();
        if (context.IsDamaged)
        {
            SwitchState(context.KnockbackState);
        }
        else if (context.IsGrounded)
        {
            SwitchState(context.GroundedState);
        }
    }

    public override void EnterState()
    {
        Debug.Log("HELLO I AM FALLING");
        //throw new System.NotImplementedException();
        context.animator.SetBool("InAir", true);
        InitializeSubState();
    }

    public override void ExitState()
    {
        context.animator.SetBool("InAir", false);
        Physics2D.gravity = new(0, -9.8f);
        //throw new System.NotImplementedException();
    }

    public override void FixedUpdateState()
    {
        //throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        //SetSubState(context.MoveState);
        if (Mathf.Abs(context.rb.velocity.x) >= 0.5)
        {
            SetSubState(context.MoveState);
            //SetSubState(context.IdleState);

        }
        else
        {
            SetSubState(context.IdleState);
        }
    }

    public override void UpdateState()
    {
        //Debug.Log("HELLO WORLD");

        //Debug.Log(context.animator.GetBool("InAir"));
        Physics2D.gravity = new(0, context.FallSpeed);
        CheckSwitchStates();

        //context.rb.velocity = Vector2.Lerp(context.rb.velocity, new Vector2(context.rb.velocity.x, MovementStats.fallSpeed), Utils.GetInterpolant(10f));
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
