using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MovementStats", order = 1)]

public class MovementStats
{
    public static float groundPoundXSpeed = 0f;
    public static float groundPoundYSpeed = Physics.gravity.y * 7;

    public static float moveSpeed =10f;
    public static float moveSpeedReduced = 0.5f;

    //intended to be able to change move speed in air.
    public static float CoyoteTime = 0.08f;
    public static float jumpMoveSpeed = 1f;
    //Force Multiplier for jumping.
    public static float jumpSpeed = 18f;
    public static float fallSpeed = Physics.gravity.y * 10f;
    public static float maxFallSpeed = 40f;
    //Controls the time for how long the player can jump.
    public static float maxJumpHeight = 0.3f;

    //backflip
    public static float backflipVerticalSpeed = 50f;
    public static float backflipHorizontalSpeed = 50f;
    public static float maxBackflipHeight = 0.3f;

    //slide
    public static float slideSpeedX = 20f;
    public static float slideDuration = .50f;
    public static float slideVerticalBounce = 18f;
    public static float slideVerticalBounceDuration = 0.1f;
    public static float slideFallSpeed = Physics.gravity.y * 2.5f;
    public static float slideCooldowntimer = 0.04f;


    //Wall Slide Speed
    public static float wallSlideSpeed = -5f;

    //Slashing Variabels
    public static float slashDuration = 1.10f;
    public static float baseSlashDamage = 1f;

    //Bouncing
    public static float maxGPBounceHeight = 0.5f;
    public static float bounceSpeed = 15f;

    //Walljumping
    public static float maxWallJumpVerticalForce = 0.025f;
    public static float maxWallJumpHorizontalForce = 0.067f;
    public static float WallJumpArrestMovementTimer = 0.17f;
    public static float WallJumpDuration = 0.2f;

    //Dash
    public static float dashDuration = 0.18f;
    public static float dashSpeed = 30f;
    public static float DashCooldown = 0.35f;
}
