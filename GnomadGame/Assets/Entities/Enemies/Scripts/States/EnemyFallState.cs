using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gnomad.Utils;
public class EnemyFallState : EnemyBaseState
{

    Vector2 currentVelocity = Vector2.zero;

    public EnemyFallState(EnemyStateMachine esm) : base(esm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        if (context.IsDamaged)
        {
            context.IsVolleyed = true;
            SwitchState(context.KnockbackState);
        }
        else if (context.IsGrounded)
        {
            SwitchState(context.GroundedState);
        }
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        if (!context.animator.GetBool("InAir"))
        {
            context.animator.SetTrigger("InAirTrigger");
            context.animator.SetBool("InAir", true);
        }
        currentVelocity = context.rb.velocity;
        InitializeSubState();
    }

    public override void ExitState()
    {
        context.animator.SetBool("InAir", false);
        Physics2D.gravity = new(0, -9.8f);
        SetSubState(context.IdleState);
        //throw new System.NotImplementedException();
    }

    public override void FixedUpdateState()
    {
        //throw new System.NotImplementedException();
        var nextXVel = Mathf.Lerp(currentVelocity.x, 0f, Utils.GetInterpolant(10));
        context.rb.velocity = new(nextXVel, context.rb.velocity.y + (MovementStats.fallSpeed * Time.fixedDeltaTime * 1.25f));
        currentVelocity = new Vector3(nextXVel, currentVelocity.y);
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
            SetSubState(null);
        }
    }

    public override void UpdateState()
    {
        //Debug.Log("HELLO WORLD");

        //Debug.Log(context.animator.GetBool("InAir"));
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
