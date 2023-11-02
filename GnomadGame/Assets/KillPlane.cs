using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health h;
        collision.TryGetComponent<Health>(out h);
        if (h != null)
        {
            h.Die();
        }
    }
}
