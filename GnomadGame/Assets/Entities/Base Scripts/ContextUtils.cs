using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public static class ContextUtils
{

    public static bool CheckIfGrounded(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);

        if (collision.transform.tag != "Ground")
        {
            return false;
        }
        foreach (var contact in contacts)
        {
            float angle = Vector2.Angle(Vector2.up, contact.normal);
            angle = Mathf.RoundToInt(angle);
            if (angle <= 80f)
            {
                return true;
            }
        }
        return false;
    }
    public static bool CheckIfGrounded(Collision2D collision, Collider2D collider)
    {

        Vector3 extents = collider.bounds.extents;
        Vector3 position = collider.transform.position;
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);

        if (collision.transform.tag != "Ground")
        {
            return false;
        }
        foreach (var contact in contacts)
        {
            float angle = Vector2.Angle(Vector2.up, contact.normal);
            angle = Mathf.RoundToInt(angle);
            if (angle <= 80f)
            {
                return true;
            }
        }

        LayerMask layerMask = LayerMask.GetMask("Ground");

        if (collider.IsTouchingLayers(layerMask))
        {
            return true;
        }
        return false;
    }
}
