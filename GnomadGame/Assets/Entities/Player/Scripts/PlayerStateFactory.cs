public class PlayerStateFactory
{
    PlayerStateMachine context;
    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        context = currentContext;
    }

    public PlayerBaseState Idle()
    {
        return new PlayerIdleState();
    }
    public PlayerBaseState Run()
    {
        return new PlayerRunState();
    }
    public PlayerBaseState Fall()
    {
        return new PlayerFallState();
    }
    public PlayerBaseState Jump()
    {
        return new PlayerJumpState();
    }

    public PlayerBaseState Grounded()
    {
        return new PlayerGroundedState();
    }

    public PlayerBaseState GroundPound()
    {
        return new PlayerGroundPoundState();
    }
}
