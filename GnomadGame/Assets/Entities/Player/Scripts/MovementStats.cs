using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MovementStats", order = 1)]

public class MovementStats
{
    public static float groundPoundXSpeed = 0f;
    public static float groundPoundYSpeed = Physics.gravity.y * 7;

    public static float moveSpeed = 9.5f;
    public static float moveSpeedReduced = 0.5f;

    //intended to be able to change move speed in air.
    public static float jumpMoveSpeed = 1f;
    //Force Multiplier for jumping.
    public static float jumpSpeed = 17f;
    public static float fallSpeed = Physics.gravity.y * 10f;

    //Controls the time for how long the player can jump.
    public static float maxWallJumpHeight = 0.08f;
    public static float maxJumpHeight = 0.4f;



    public static float slideSpeedX = 15f;
    public static float slideDuration = .35f;
    public static float slideFallSpeed = Physics.gravity.y * 2f;
    public static float wallSlideSpeed = -5f;

    //Slashing Variabels
    public static float slashDuration = .15f;

    //Bouncing
    public static float maxGPBounceHeight = 0.5f;
    public static float bounceSpeed = 15f;
}
