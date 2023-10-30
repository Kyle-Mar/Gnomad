using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StaticHintController : MonoBehaviour
{
    private void Awake()
    {
        transform.GetComponent<TextMeshPro>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.GetComponent<TextMeshPro>().enabled = true;
    }
}
