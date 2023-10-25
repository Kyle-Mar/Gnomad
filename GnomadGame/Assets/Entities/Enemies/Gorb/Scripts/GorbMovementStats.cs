using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorbStats : EnemyMovementStats
{
    public new const float moveSpeed = 5f;
    public new const float chargeSpeed = 12.5f;
    public new const float moveSpeedReduced = 0.5f;

    //intended to be able to change move speed in air.
    public new const float jumpMoveSpeed = 1f;

    //Force Multiplier for jumping.
    public new const float jumpSpeed = 10f;
    public new static float fallSpeed = Physics.gravity.y * 15f;

    // Knockback stats
    public new static float knockbackSpeed = 1f;
    public new static float knockbackXConst = 0.85f;
    public new static float knockbackYConst = 0.5f;
    public new static float knockbackTimer = 0.3f;

    //Controls the time for how long the player can jump.
    public new const float maxJumpHeight = 0.4f;

    //Slashing Variabels
    public new const float attackDuration = 1.5f;
    public new const float attackCooldown = 2.5f;
    public new const float attackDamage = 1f;

}
