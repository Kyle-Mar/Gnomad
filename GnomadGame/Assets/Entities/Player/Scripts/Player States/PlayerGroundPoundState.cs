using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gnomad.Utils;

public class PlayerGroundPoundState : PlayerBaseState
{
    public PlayerGroundPoundState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        context.CheckIfGrounded();
        if (context.IsGPBounceQueued)
        {
            SwitchState(context.GPBounceState);
        }
        else if (context.IsGrounded)
        {
            SwitchState(context.GroundedState);
        }
    }

    public override void EnterState()
    {
        InitializeSubState();
        context.SpriteRenderer.flipY = true; //will be changed when animations are added
        context.HatSpriteRenderer.enabled = false; //will be changed when animations are added
        context.ConsumeJumpBuffer();
        context.GroundPoundCollider.gameObject.SetActive(true);
        context.Animator.SetTrigger("GroundPoundTrigger");
    }

    public override void ExitState()
    {
        //Debug.Log("exiting");
        context.SpriteRenderer.flipY = false; //will be changed when animations are added
        context.HatSpriteRenderer.enabled = true; //will be changed when animations are added
        context.rb.velocity = new(0, 0);
        context.GroundPoundCollider.gameObject.SetActive(false);
        context.Animator.SetTrigger("StopGroundPoundTrigger");

        //throw new System.NotImplementedException();
    }

    public override void FixedUpdateState()
    {
       // throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        SetSubState(null);
        //Debug.Log(context.IdleState.CurrentSubState());
        //throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        //Debug.Log(currentSubState);
        context.rb.velocity = Vector2.Lerp(context.rb.velocity,
                              new(MovementStats.groundPoundXSpeed, MovementStats.groundPoundYSpeed),
                              Utils.GetInterpolant(100f));
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
