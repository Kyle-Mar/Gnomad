using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementStats
{
    public static float moveSpeed = 5f;
    public static float chargeSpeed = 12.5f;
    public static float moveSpeedReduced = 0.5f;

    //intended to be able to change move speed in air.
    public static float jumpMoveSpeed = 1f;

    //Force Multiplier for jumping.
    public static float jumpSpeed = 17f;
    public static float fallSpeed = Physics.gravity.y * 15f;

    // Knockback stats
    public static float knockbackTimer = 0.4f;
    public static float knockbackSpeed = 1f;
    public static float knockbackXConst = 5f;
    public static float knockbackYConst = 1f;

    //Controls the time for how long the player can jump.
    public static float maxJumpHeight = 0.4f;

    //Slashing Variabels
    public static float attackDuration = 1.5f;
    public static float attackCooldown = 2.5f;
    public static int attackDamage = 20;

}
