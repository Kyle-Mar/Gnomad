using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void CheckSwitchStates()
    {
        if (context.Controls.Player.Move.ReadValue<Vector2>().x != 0)
        {
            SwitchState(context.RunState);
        }
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override void FixedUpdateState()
    {
        //throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        context.rb.velocity = new(0, context.rb.velocity.y);// this generates warnings, but will not be called when death state is added so no need to fix
        // do nothing, maybe play idle anim later.
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
