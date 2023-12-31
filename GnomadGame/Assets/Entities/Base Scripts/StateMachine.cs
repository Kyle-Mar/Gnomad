using UnityEngine;
using UnityEngine.Assertions;


public class StateMachine : MonoBehaviour
{

    [SerializeField]protected BaseState currentState;
    public virtual BaseState CurrentState { get { return currentState; } set { currentState = value; } }

    private void Awake()
    {
        Assert.IsNotNull(currentState);
        currentState.EnterState();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateStates();
    }

    void Update()
    {
        currentState.UpdateStates();
    }


}
