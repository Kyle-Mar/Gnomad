using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine psm) : base(psm)
    {
    }

    Vector2 inputVec;

    public override void CheckSwitchStates()
    {
        if(context.Controls.Player.Move.ReadValue<Vector2>().x == 0)
        {
            SwitchState(context.IdleState);
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

    public override void FixedUpdateState()
    {

        Vector2 movementVector = new(0, 0);
        movementVector.x = inputVec.x * context.CurrentMoveSpeed;
        movementVector.y = context.rb.velocity.y;

        movementVector.x = Mathf.Lerp(context.rb.velocity.x, movementVector.x, Utils.GetInterpolant(100f));

        //.Log(context.CurrentMoveSpeed);
        context.rb.velocity = movementVector;

    }

    public override void InitializeSubState()
    {
        //throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        inputVec = context.Controls.Player.Move.ReadValue<Vector2>();

        if (inputVec.x <= -0.001 && context.SpriteRenderer.flipX)
        { context.FlipSprite(); }
        else if (inputVec.x >= 0.001 && !context.SpriteRenderer.flipX)
        { context.FlipSprite(); }

        if(inputVec.x == 0)
        {
            context.rb.velocity = new(0, context.rb.velocity.y);
        }
        else if (context.IsGrounded)
        {
            ParticleSystem run_particle_object = Object.Instantiate(context.WalkParticles, context.Feet.position, Quaternion.identity);
        }
        
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
