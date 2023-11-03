using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for changing to an empty substate.
public class PlayerNotAttackingState : PlayerBaseState
{

    float delayTillNextAttack = 0f;


    public PlayerNotAttackingState(PlayerStateMachine psm) : base(psm)
    {
    }

    public override void CheckSwitchStates()
    {
        if (context.Controls.Player.Slash.WasPressedThisFrame() && context.CheckCanSlash() && delayTillNextAttack <= 0f)
        {
            SwitchState(context.SlashState);
        }
    }

    public override void InitializeSubState()
    {
    }
    
    public override void EnterState()
    {
        delayTillNextAttack = 0.3f;
        CheckSwitchStates();//handles the case that slash is preformed this very frame
                            //possibly not necessary depending on when update is called. Idk ~Elijah
    }

    public override void ExitState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void UpdateState()
    {
        delayTillNextAttack -= Time.deltaTime;
        CheckSwitchStates();
    }

}
