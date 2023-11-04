using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRefill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) { return; }
        if (!collision.gameObject.CompareTag("Player")) { return; }

        collision.transform.GetComponent<Flask>().RefillFlask();

    }
}

