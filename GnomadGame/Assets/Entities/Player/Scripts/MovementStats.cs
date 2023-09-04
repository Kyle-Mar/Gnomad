using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MovementStats", order = 1)]

public class MovementStats
{
    public static float groundPoundXSpeed = 0f;
    
    public static float groundPoundYSpeed = Physics.gravity.y * 8;

    public static float moveSpeed = 5;
    //intended to be able to change move speed in air.
    public static float jumpMoveSpeed = 1f;
    //Controls the time for how long the player can jump.
    public static float maxJumpHeight = 0.15f;
    //Force Multiplier for jumping.
    public static float jumpSpeed=12;

    public static float fallSpeed = Physics.gravity.y * 5;

    public static float slideSpeedX = 10f;

    public static float slideDuration = .5f;

}
