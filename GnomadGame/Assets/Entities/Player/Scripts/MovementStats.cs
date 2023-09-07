using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MovementStats", order = 1)]

public class MovementStats
{
    public static float groundPoundXSpeed = 0f;
    
    public static float groundPoundYSpeed = Physics.gravity.y * 7;

    public static float moveSpeed = 9.5f;
    //intended to be able to change move speed in air.
    public static float jumpMoveSpeed = 1f;
    //Controls the time for how long the player can jump.
    public static float maxJumpHeight = 0.3f;
    //Force Multiplier for jumping.
    public static float jumpSpeed=15;

    public static float fallSpeed = Physics.gravity.y * 5f;

    public static float slideSpeedX = 15f;

    public static float slideDuration = .35f;

    public static float slideFallSpeed = Physics.gravity.y * 2f;
}
