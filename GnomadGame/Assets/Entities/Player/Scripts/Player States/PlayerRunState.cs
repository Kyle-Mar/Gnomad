using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine psm, PlayerStateFactory psf) : base(psm, psf)
    {
    }

    public override void CheckSwitchStates()
    {
        if(context.Controls.Player.Move.ReadValue<Vector2>().x == 0)
        {
            SwitchState(factory.Idle());
        }
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        //throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        Debug.Log("RUNNING?");
        CheckSwitchStates();
        Vector2 inputVector = context.Controls.Player.Move.ReadValue<Vector2>();
        Vector2 movementVector = new(0, 0);
        movementVector.x = inputVector.x * MovementStats.moveSpeed;
        movementVector.y = context.rb.velocity.y;

        context.rb.velocity = movementVector;
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
