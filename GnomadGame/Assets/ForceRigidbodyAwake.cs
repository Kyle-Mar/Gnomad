using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceRigidbodyAwake : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.WakeUp();
    }
}
