using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for changing to an empty substate.
public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState(PlayerStateMachine psm) : base(psm)
    {
        isRootState = true;
    }

    public override void CheckSwitchStates()
    {
        //throw new System.NotImplementedException();
    }

    public override void EnterState()
    {
        //throw new System.NotImplementedException();
        context.GetComponentInChildren<Health>().enabled = false;
        context.HatSpriteRenderer.gameObject.SetActive(false);
        context.SpriteRenderer.gameObject.SetActive(false);
        context.col.enabled = false;
        context.rb.bodyType = RigidbodyType2D.Static;
        context.SlashCollider.enabled = false;
        context.SlideCollider.enabled = false;
        context.GroundPoundCollider.enabled = false;
        context.HurtBox.enabled = false;
        GameObject deathParts = Object.Instantiate(context.DeathPartsPrefab, context.transform.position, Quaternion.identity);
    }

    public override void ExitState()
    {
        //throw new System.NotImplementedException();
    }

    public override void FixedUpdateState()
    {
       // throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        SetSubState(null);
    }

    public override void UpdateState()
    {
        //throw new System.NotImplementedException();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
