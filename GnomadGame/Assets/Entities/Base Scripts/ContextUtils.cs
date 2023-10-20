using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class ContextUtils
{
    public static bool CheckIfGrounded(ref Collider2D col, UnityEngine.Transform transform, ref ContactFilter2D floorContactFilter)
    {
        //Debug.Log(col.bounds.extents);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, col.bounds.extents * 2, 0f, -transform.up, 0.5f, floorContactFilter.layerMask);

        //Debug.Log(hit.collider.name);
        //Debug.DrawRay(transform.position, -transform.up * col.bounds.extents.y * 2, Color.blue, .5f);
        
        if (hit.collider && col.IsTouching(hit.collider, floorContactFilter))
        {
            return true;
            //wallSlideExpired = false;
        }
        else
        {
            return false;
        }
    }
}
