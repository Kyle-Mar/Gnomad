public class PlayerStateFactory
{
    PlayerStateMachine context;
    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        context = currentContext;
    }

    public PlayerBaseState Idle()
    {
        return new PlayerIdleState(context, this);
    }
    public PlayerBaseState Run()
    {
        return new PlayerRunState(context, this);
    }
    public PlayerBaseState Fall()
    {
        return new PlayerFallState(context, this);
    }
    public PlayerBaseState Jump()
    {
        return new PlayerJumpState(context, this);
    }

    public PlayerBaseState Grounded()
    {
        return new PlayerGroundedState(context, this);
    }

    public PlayerBaseState GroundPound()
    {
        return new PlayerGroundPoundState(context, this);
    }
}
