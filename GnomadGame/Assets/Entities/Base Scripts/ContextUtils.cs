using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
            if (angle <= 45f)
            {
                return true;
            }
        }
        return false;
    }
}
