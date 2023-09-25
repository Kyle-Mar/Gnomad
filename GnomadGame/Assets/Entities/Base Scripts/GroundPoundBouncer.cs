using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class GroundPoundBouncer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) { Debug.Log("Ground Pound Bounce Collider == null"); return; }
        if (collision.gameObject.name != "Ground Pound Collider") { return; }
        Debug.Log(collision.gameObject.name);
        GameObject player = collision.gameObject.transform.parent.gameObject;
        player.GetComponent<PlayerStateMachine>().IsGPBounceQueued = true;

    }
}
