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
    [SerializeField] float smoothingFactor;
    [SerializeField] float horizontalAnticipation;
    [SerializeField] float verticalAnticipation;
    [SerializeField] Vector3 originalPosition;
    [SerializeField] float offsetY;
    [SerializeField] Vector3 desiredPosition;
    [SerializeField] Vector3 currentAnticipation;
    [SerializeField] CompositeCollider2D boundingCollider;

    Vector3 bottomLeft;
    Vector3 bottomRight;
    Vector3 topLeft;
    Vector3 topRight;
    Vector3 middleBottom;
    Vector3 middleTop;
    Vector3 middleLeft;
    Vector3 middleRight;
    Camera mainCamera;

    void Start()
    {
        Assert.IsNotNull(playerTransform);
        Assert.IsNotNull(playerRB);
        desiredPosition = GetCameraPosFromPlayerPos();
        currentAnticipation = GetAnticipationVector();
        originalPosition = transform.position;
        mainCamera = GetComponent<Camera>();
        CalculateCollisionPoints();
    }

    private void OnEnable()
    {
        LevelManager.onEnterNewRoom += OnEnterNewRoom;
    }

    private void OnDisable()
    {
        LevelManager.onEnterNewRoom -= OnEnterNewRoom;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (psm.CurrentState != psm.GroundedState)
        {
            originalPosition = new(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        }
        currentAnticipation = Vector3.Lerp(currentAnticipation, GetAnticipationVector(), Utils.GetInterpolant(smoothingFactor));
        desiredPosition = GetCameraPosFromPlayerPos() + currentAnticipation;
        desiredPosition.z = originalPosition.z;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Utils.GetInterpolant(smoothingFactor));
    }

    void Update()
    {
        if(boundingCollider)
        Debug.Log(boundingCollider.OverlapPoint(GetPointBoundsAligned(middleBottom)));
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
    void CalculateCollisionPoints()
    {
        bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.farClipPlane));
        bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.farClipPlane));
        topLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.farClipPlane));
        topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.farClipPlane));

        // Calculate vectors for the four edges (midpoints of the corners)
        middleBottom = Vector3.Lerp(bottomLeft, bottomRight, 0.5f);
        middleTop = Vector3.Lerp(topLeft, topRight, 0.5f);
        middleLeft = Vector3.Lerp(bottomLeft, topLeft, 0.5f);
        middleRight = Vector3.Lerp(bottomRight, topRight, 0.5f);
    }

    Vector3 GetPointBoundsAligned(Vector3 point)
    {
        mainCamera.ViewportToWorldPoint(point);
        point.z = boundingCollider.transform.position.z;
        return point;
    }
    
    public void OnEnterNewRoom(CompositeCollider2D boundingCollider)
    {
        Debug.Log("Hello from Camera System");
    }
}
