public abstract class FSMState
{
    public virtual void Enter() {}
    public virtual FSMState Execute() => this;
    public virtual void Exit() {}
}

public class FiniteStateMachine : FSMState
{
    public FSMState state { get; protected set; }

    public FiniteStateMachine(FSMState initialState)
    {
        state = initialState;
    }

    public override void Enter()
    {
        state.Enter();
    }

    public override FSMState Execute()
    {
        FSMState previousState = state;
        state = state.Execute();
        if (state != previousState)
        {
            previousState.Exit();
            state.Enter();
        }
        return this;
    }

    public override void Exit()
    {
        state.Exit();
    }

    public void TransitionTo(FSMState state)
    {
        this.state.Exit();
        this.state = state;
        state.Enter();
    }
}