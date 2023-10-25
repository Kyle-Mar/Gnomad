using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorbBuilder : MonoBehaviour
{
    EnemyStateMachine esm;

    public GorbChargeAttackState attackState;
    public EnemyNotAttackState notAttackState;
    public EnemyMoveState moveState;
    public EnemyIdleState idleState;
    public EnemyGroundedState groundedState;
    public EnemyFallState fallState;

    // Start is called before the first frame update
    void Start()
    {
        esm = GetComponent<EnemyStateMachine>();

        attackState = new GorbChargeAttackState(esm);
        notAttackState = new EnemyNotAttackState(esm);
        moveState = new EnemyMoveState(esm);
        idleState = new EnemyIdleState(esm);    
        groundedState = new EnemyGroundedState(esm);
        fallState = new EnemyFallState(esm);

        esm.BuildAttackState(attackState)
            .BuildNotAttackState(notAttackState)
            .BuildGroundedState(groundedState)
            .BuildIdleState(idleState)
            .BuildMoveState(moveState)
            .BuildFallState(fallState);
    
    }
}
