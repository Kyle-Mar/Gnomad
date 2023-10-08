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

        context.CheckIfGrounded();
        if (!context.IsGrounded)
        {
            // Uncomment once FallState is implemented


            //SwitchState(context.FallState);
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        Debug.Log("HELLO WORLD ENEMY AM GROUNDED!");
        //context.rb.velocity = new(context.rb.velocity.x, 0);
        //Object.Instantiate(context.LandParticles, context.Feet.position, Quaternion.identity);
    }

    public override void ExitState()
    {
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
        if (context.targetObject != null)
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
