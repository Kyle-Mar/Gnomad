using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class WindController : MonoBehaviour
{
    //private WindVelocityController windVelocityController;
    [SerializeField] SpriteRenderer[] effectedSprites;
    
    private Material material;

    private bool easeInCoroutineRunning;
    private bool easeOutCoroutineRunning;

    private int externalInfluence = Shader.PropertyToID("_ExternalInfluence");
    [Range(0f, 1f)] public float ExternalInfluenceStrength = 0.25f;

    private float startingXVelocity;
    private float velocityLastFrame;

    public float EaseInTime = 0.15f;
    public float EaseOutTime = 0.15f;
    public float VelocityThreshold = 5f;

    private void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        Assert.IsNotNull(material);
        startingXVelocity = material.GetFloat(externalInfluence);
        easeInCoroutineRunning = false;
        easeOutCoroutineRunning = false;
        velocityLastFrame = 0;
    }

    private IEnumerator EaseIn(float xVelocity)
    {
        easeInCoroutineRunning = true;

        float elapsedTime = 0f;
        while(elapsedTime < EaseInTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedAmount = Mathf.Lerp(startingXVelocity, xVelocity, (elapsedTime / EaseInTime));
            InfluenceGrass(material, lerpedAmount);
            yield return null;
        }

        easeInCoroutineRunning = false;
    }

    private IEnumerator EaseOut()
    {
        easeOutCoroutineRunning = true;

        float currentXInfluence = material.GetFloat(externalInfluence);

        float elapsedTime = 0f;
        while (elapsedTime < EaseOutTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedAmount = Mathf.Lerp(currentXInfluence, startingXVelocity, (elapsedTime / EaseOutTime));
            InfluenceGrass(material, lerpedAmount);
            yield return null;
        }

        easeOutCoroutineRunning = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
            if (!easeInCoroutineRunning &&
                Mathf.Abs(playerRB.velocity.x) > Mathf.Abs(VelocityThreshold))
            {
                StartCoroutine(EaseIn(playerRB.velocity.x * ExternalInfluenceStrength));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(EaseOut());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRB = collision.gameObject.GetComponent<Rigidbody2D>();

            if (Mathf.Abs(velocityLastFrame) > Mathf.Abs(VelocityThreshold) &&
                Mathf.Abs(playerRB.velocity.x) < Mathf.Abs(VelocityThreshold))
            {
                StartCoroutine(EaseOut());
            }
            else if (Mathf.Abs(velocityLastFrame) < Mathf.Abs(VelocityThreshold) &&
                Mathf.Abs(playerRB.velocity.x) < Mathf.Abs(VelocityThreshold))
            {
                StartCoroutine(EaseIn(playerRB.velocity.x * ExternalInfluenceStrength));
            }
            else if (!easeInCoroutineRunning && !easeOutCoroutineRunning &&
                Mathf.Abs(playerRB.velocity.x) > Mathf.Abs(VelocityThreshold))
            {
                InfluenceGrass(material, playerRB.velocity.x * ExternalInfluenceStrength);
            }
            velocityLastFrame = playerRB.velocity.x;
        }
    }

    public void InfluenceGrass(Material mat, float XVelocity)
    {
        mat.SetFloat(externalInfluence, XVelocity);
        foreach (SpriteRenderer s in effectedSprites) {
            if(s != null)
            {
                s.material.SetFloat(externalInfluence, XVelocity);
            }
        }
    }

}
