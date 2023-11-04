using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveHatTrigger : MonoBehaviour
{
    [SerializeField] Sprite hatNeutral;
    [SerializeField] Sprite hatOff;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision == null) { return; }
        if (!collision.gameObject.CompareTag("Player")) { return; }

        collision.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = hatNeutral;
        collision.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = hatOff;

    }
}
