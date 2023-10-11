using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindExternalVelocityTrigger : MonoBehaviour
{
    private WindVelocityController windVelocityController;
    [SerializeField]private GameObject player;
    private Material material;
    private Rigidbody2D playerRB;

    private bool easeInCoroutineRunning;
    private bool easeOutCoroutineRunning;

    private int externalInfluence = Shader.PropertyToID("_ExternalInfluence");

    private float startingXVelocity;
    private float velocityLastFrame;

    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        windVelocityController = GetComponentInParent<WindVelocityController>();

        material = GetComponent<SpriteRenderer>().material;
        startingXVelocity = material.GetFloat(externalInfluence);
        easeInCoroutineRunning = false;
        easeOutCoroutineRunning = false;
        velocityLastFrame = 0;
    }

    private IEnumerator EaseIn(float xVelocity)
    {
        easeInCoroutineRunning = true;

        float elapsedTime = 0f;
        while(elapsedTime < windVelocityController.EaseInTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedAmount = Mathf.Lerp(startingXVelocity, xVelocity, (elapsedTime / windVelocityController.EaseInTime));
            windVelocityController.InfluenceGrass(material, lerpedAmount);
            yield return null;
        }

        easeInCoroutineRunning = false;
    }

    private IEnumerator EaseOut()
    {
        easeOutCoroutineRunning = true;

        float currentXInfluence = material.GetFloat(externalInfluence);

        float elapsedTime = 0f;
        while (elapsedTime < windVelocityController.EaseOutTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedAmount = Mathf.Lerp(currentXInfluence, startingXVelocity, (elapsedTime / windVelocityController.EaseOutTime));
            windVelocityController.InfluenceGrass(material, lerpedAmount);
            yield return null;
        }

        easeOutCoroutineRunning = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            if (!easeInCoroutineRunning &&
                Mathf.Abs(playerRB.velocity.x) > Mathf.Abs(windVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseIn(playerRB.velocity.x * windVelocityController.ExternalInfluenceStrength));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        print(collision.gameObject);
        if (collision.gameObject == player)
        {
            StartCoroutine(EaseOut());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            if (Mathf.Abs(velocityLastFrame) > Mathf.Abs(windVelocityController.VelocityThreshold) &&
                Mathf.Abs(playerRB.velocity.x) < Mathf.Abs(windVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseOut());
            }
            else if (Mathf.Abs(velocityLastFrame) < Mathf.Abs(windVelocityController.VelocityThreshold) &&
                Mathf.Abs(playerRB.velocity.x) < Mathf.Abs(windVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseIn(playerRB.velocity.x * windVelocityController.ExternalInfluenceStrength));
            }
            else if (!easeInCoroutineRunning && !easeOutCoroutineRunning &&
                Mathf.Abs(playerRB.velocity.x) > Mathf.Abs(windVelocityController.VelocityThreshold))
            {
                windVelocityController.InfluenceGrass(material, playerRB.velocity.x * windVelocityController.ExternalInfluenceStrength);
            }
            velocityLastFrame = playerRB.velocity.x;
        }
    }

}
