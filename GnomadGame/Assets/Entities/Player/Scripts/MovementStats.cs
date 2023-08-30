using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MovementStats", order = 1)]

public class MovementStats
{
    public static float moveSpeed = 5;
    public static float moveInertia = 1;
    //intended to be able to change move speed in air.
    public static float jumpMoveSpeed = 1f;
    //Controls the time for how long the player can jump.
    public static float maxJumpHeight = 0.1f;
    //Force Multiplier for jumping.
    public static float jumpSpeed=15;
}
