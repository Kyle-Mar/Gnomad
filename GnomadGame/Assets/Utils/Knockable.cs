using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Knockable : MonoBehaviour, IKnockable
{

    [SerializeField] Vector3 KBDir = Vector3.zero;

    public Vector3 KBDirection { get => KBDir; set => KBDir = value; }

    // Gets declared inside OnAwake function on parent object (ESM OnAwake(), PSM OnAwake(), etc)
    public delegate void OnKnockback(Vector3 dir);
    public OnKnockback onKnockback;

    public void Knockback(Vector3 dir)
    {
        KBDir = dir;
        if (onKnockback.GetInvocationList().Length > 0)
        {
            onKnockback?.Invoke(KBDir);
        }
    }

}
