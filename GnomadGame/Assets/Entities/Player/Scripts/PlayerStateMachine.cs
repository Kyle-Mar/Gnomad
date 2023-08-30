using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{

    PlayerBaseState currentState;
    PlayerStateFactory states;

    private void Awake()
    {
        states = new(this);
        currentState = states.Grounded();
        currentState.EnterState();
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
