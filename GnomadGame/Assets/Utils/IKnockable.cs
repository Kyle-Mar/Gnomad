using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface IKnockable
{ 
    

    public Vector3 KBDirection { get; set; }

    public void Knockback(Vector3 dir)
    {
        KnockableExtensions.Knockback(this, dir);
    }
}

// Handles Declarations for Knockback to make sure it is usable and not broken
// Also used for other reasons
public static class KnockableExtensions
{

    public static void Knockback(this IKnockable kb, Vector3 dir)
    {
        kb.KBDirection = dir;
    }
}
