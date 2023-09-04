using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine psm, PlayerStateFactory psf) : base(psm, psf)
    {
    }

    public override void CheckSwitchStates()
    {
        //Debug.Log(Input.GetAxis("Horizontal"));
        if (Input.GetAxis("Horizontal") != 0)
        {
            SwitchState(factory.Run());
        }
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        CheckSwitchStates();
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
