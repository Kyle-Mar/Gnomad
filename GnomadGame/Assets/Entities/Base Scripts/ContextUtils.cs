using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class ContextUtils
{
    public static bool CheckIfGrounded(ref Collider2D col, UnityEngine.Transform transform, ref ContactFilter2D floorContactFilter)
    {
        //Debug.Log(col.bounds.extents);
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position - (transform.up*0.5f), col.bounds.extents * 2, 0f, floorContactFilter.layerMask);
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, col.bounds.extents * 2, 0f, -transform.up, col.bounds.extents.y*2, floorContactFilter.layerMask);
        //Debug.Log(hit.collider.name);
        //Debug.DrawRay(transform.position, -transform.up * col.bounds.extents.y * 2, Color.blue, .5f);
        
        if(hits.Length <= 0)
        {
            return false;
        }

        foreach(var hit in hits)
        {
            if (col.IsTouching(hit))
            {
                return true;
            }
        }
        return false;
    }

    public static bool NewCheckIfGrounded(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);

        if (collision.transform.tag != "Ground")
        {
            return false;
        }
        foreach (var contact in contacts)
        {
            float angle = Vector2.SignedAngle(Vector2.up, contact.normal);
            angle = Mathf.RoundToInt(angle);
            if (angle == 0f)
            {
                return true;
            }
        }
        return false;
    }
}
