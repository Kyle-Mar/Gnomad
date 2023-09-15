using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundPoundState : PlayerBaseState
{
    public PlayerGroundPoundState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        context.CheckIfGrounded();
        if (context.IsGrounded)
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
    }

    public override void ExitState()
    {
        context.SpriteRenderer.flipY = false; //will be changed when animations are added
        context.HatSpriteRenderer.enabled = true; //will be changed when animations are added
        context.rb.velocity = new(0, 0);
        context.GroundPoundCollider.gameObject.SetActive(false);
        //throw new System.NotImplementedException();
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
        CheckSwitchStates();
        context.rb.velocity = Vector2.Lerp(context.rb.velocity,
                              new(MovementStats.groundPoundXSpeed, MovementStats.groundPoundYSpeed),
                              Utils.GetInterpolant(100f));
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
