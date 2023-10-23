using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Gnomad.Utils;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] PlayerStateMachine psm;
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] Camera camera;
    [SerializeField] CompositeCollider2D boundingArea;
    [SerializeField] float smoothingFactor;
    [SerializeField] float horizontalAnticipation;
    [SerializeField] float verticalAnticipation;
    [SerializeField] Vector3 originalPosition;
    [SerializeField] float offsetY;
    [SerializeField] Vector2 yDeadzone;
    [SerializeField] Vector3 desiredPosition;
    [SerializeField] Vector3 currentAnticipation;
    void Start()
    {
        Assert.IsNotNull(playerTransform);
        Assert.IsNotNull(playerRB);
        desiredPosition = GetCameraPosFromPlayerPos();
        currentAnticipation = GetAnticipationVector();
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (psm.CurrentState != psm.GroundedState)
        {
            //originalPosition = new(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        }
        currentAnticipation = Vector3.Lerp(currentAnticipation, GetAnticipationVector(), Utils.GetInterpolant(smoothingFactor));
        desiredPosition = GetCameraPosFromPlayerPos() + currentAnticipation;
        desiredPosition.z = originalPosition.z;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Utils.GetInterpolant(smoothingFactor));
    }

    void Update()
    {
        UpdateYPos();
    }

    void UpdateYPos()
    {
        Vector3 playerPos = camera.WorldToViewportPoint(playerTransform.position);
        if (playerPos.y > yDeadzone.x && playerPos.y < yDeadzone.y)
        {
            return;
        }
        originalPosition = new(playerTransform.position.x, playerTransform.position.y, transform.position.z);
    }

    Vector3 GetCameraPosFromPlayerPos()
    {
        return new Vector3(playerTransform.position.x, offsetY + originalPosition.y, transform.position.z);
    }

    Vector3 GetAnticipationVector()
    {
        var anticipationVector = new Vector3(playerRB.velocity.x, playerRB.velocity.y, 0);
        if (playerRB.velocity.magnitude > 1)
        {
            anticipationVector.Normalize();
            anticipationVector = Vector3.Scale(anticipationVector, new Vector3(horizontalAnticipation, verticalAnticipation, 0));
        }
        Debug.Log(anticipationVector);
        return anticipationVector;
    }
}
