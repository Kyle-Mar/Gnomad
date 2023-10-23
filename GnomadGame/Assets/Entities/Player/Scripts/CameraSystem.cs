using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Gnomad.Utils;

public class CameraSystem : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] bool doRenderSpheres;
    [Header("Player Info")]
    [SerializeField] Transform playerTransform;
    [SerializeField] PlayerStateMachine psm;
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] CompositeCollider2D boundingArea;
    [Header("Camera Details")]
    [SerializeField] Camera mainCamera;
    [SerializeField] Vector3 originalPosition;
    [SerializeField] Vector3 desiredPosition;
    [SerializeField] Vector3 currentAnticipation;
    [SerializeField] Vector3 desiredDelta;
    [Header("Camera Settings")]
    [SerializeField] Vector2 yDeadzone;
    [SerializeField] float smoothingFactor;
    [SerializeField] float smoothingFactorAnticipationX;
    [SerializeField] float smoothingFactorAnticipationY;
    [SerializeField] float horizontalAnticipation;
    [SerializeField] float verticalAnticipation;
    [SerializeField] float fallingAnticipationMultiplier;
    [SerializeField] float offsetY;
    [Header("Current Bounding Collider")]
    [SerializeField] CompositeCollider2D boundingCollider;

    Vector3 bottomLeft;
    Vector3 bottomRight;
    Vector3 topLeft;
    Vector3 topRight;
    Vector3 middleBottom;
    Vector3 middleTop;
    Vector3 middleLeft;
    Vector3 middleRight;
    public float allowedAmountPosX;
    public float allowedAmountPosY;
    public float allowedAmountNegX;
    public float allowedAmountNegY;

    void Start()
    {
        Assert.IsNotNull(playerTransform);
        Assert.IsNotNull(playerRB);
        desiredPosition = GetCameraPosFromPlayerPos();
        currentAnticipation = GetAnticipationVector();
        originalPosition = transform.position;
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
            //originalPosition = new(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        }
        var nextAnticipation = GetAnticipationVector();
        currentAnticipation = new Vector3(
                                Mathf.Lerp(currentAnticipation.x, nextAnticipation.x, Utils.GetInterpolant(smoothingFactorAnticipationX)),
                                Mathf.Lerp(currentAnticipation.y, nextAnticipation.y, Utils.GetInterpolant(smoothingFactorAnticipationY)),
                                currentAnticipation.z
                                );
        desiredPosition = GetCameraPosFromPlayerPos() + currentAnticipation;
        desiredPosition.z = originalPosition.z;
        desiredDelta = Vector3.Lerp(transform.position, desiredPosition, Utils.GetInterpolant(smoothingFactor)) - transform.position;
        //Debug.Log(desiredDelta);
        transform.position += GetAllowedDelta(desiredDelta);
    }

    void Update()
    {
        UpdateYPos();
        


            //Debug.Log($"AM I OUT?: {!boundingCollider.OverlapPoint(GetPointBoundsAligned(middleBottom))}");
            //Debug.Log($"AM I OUT?: {!boundingCollider.OverlapPoint(GetPointBoundsAligned(middleRight))}");
            //Debug.Log($"AM I OUT?: {!boundingCollider.OverlapPoint(GetPointBoundsAligned(middleLeft))}");
            //Debug.Log($"AM I OUT?: {!boundingCollider.OverlapPoint(GetPointBoundsAligned(topRight))}");
            //Debug.Log($"AM I OUT?: {!boundingCollider.OverlapPoint(GetPointBoundsAligned(topLeft))}");
            //Debug.Log($"AM I OUT?: {!boundingCollider.OverlapPoint(GetPointBoundsAligned(bottomLeft))}");
            //Debug.Log($"AM I OUT?: {!boundingCollider.OverlapPoint(GetPointBoundsAligned(bottomRight))}");
        
    }

    void UpdateYPos()
    {
        //Vector3 playerPos = GetComponent<Camera>().WorldToViewportPoint(playerTransform.position);
        //if (playerPos.y > yDeadzone.x && playerPos.y < yDeadzone.y)
        //{
        //   return;
        //}
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
            if (playerRB.velocity.y < -45f)
            {
                anticipationVector = Vector3.Scale(anticipationVector, new Vector3(horizontalAnticipation, verticalAnticipation * fallingAnticipationMultiplier, 0));
            }
            else
            {
                anticipationVector = Vector3.Scale(anticipationVector, new Vector3(horizontalAnticipation, verticalAnticipation, 0));

            }
        }
        //Debug.Log(anticipationVector);
        return anticipationVector;
    }
    void CalculateCollisionPoints()
    {
        var zPos = Mathf.Abs(mainCamera.transform.position.z - boundingCollider.transform.position.z);
        bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, zPos));
        bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, zPos));
        topLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, zPos));
        topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, zPos));

        // Calculate vectors for the four edges (midpoints of the corners)
        middleBottom = Vector3.Lerp(bottomLeft, bottomRight, 0.5f);
        middleTop = Vector3.Lerp(topLeft, topRight, 0.5f);
        middleLeft = Vector3.Lerp(bottomLeft, topLeft, 0.5f);
        middleRight = Vector3.Lerp(bottomRight, topRight, 0.5f);
    }

    Vector3 GetPointBoundsAligned(Vector3 point)
    {
        if (doRenderSpheres)
        {
            point.z = boundingCollider.transform.position.z;
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = point;
        }
        return point;
    }

    public void OnEnterNewRoom(CompositeCollider2D boundingCollider)
    {
        if (!boundingCollider)
        {
            return;
        }
        this.boundingCollider = boundingCollider;
        Debug.Log("Hello from Camera System");
    }

    Vector3 GetAllowedDelta(Vector3 desiredDelta)
    {
        if (boundingCollider)
        {
            allowedAmountPosX = 0;
            allowedAmountPosY = 0;
            allowedAmountNegX = 0;
            allowedAmountNegY = 0;
            CalculateCollisionPoints();
            if (boundingCollider.OverlapPoint(GetPointBoundsAligned(topRight)))
            {
                allowedAmountPosY += (1f / 3);
                allowedAmountPosX += (1f / 3);
            }
            if (boundingCollider.OverlapPoint(GetPointBoundsAligned(topLeft)))
            {
                allowedAmountNegX += (1f / 3);
                allowedAmountPosY += (1f / 3);
            }
            if (boundingCollider.OverlapPoint(GetPointBoundsAligned(bottomLeft)))
            {
                allowedAmountNegX += (1f / 3);
                allowedAmountNegY += (1f / 3);
            }
            if (boundingCollider.OverlapPoint(GetPointBoundsAligned(bottomRight)))
            {
                allowedAmountPosX += (1f / 3);
                allowedAmountNegY += (1f / 3);
            }
            if (boundingCollider.OverlapPoint(GetPointBoundsAligned(middleBottom)))
            {

                allowedAmountNegY = 1;
            }
            else if(Mathf.Approximately(allowedAmountNegY, 1f / 3))
            {
                allowedAmountNegY = 0;
            }
            if (boundingCollider.OverlapPoint(GetPointBoundsAligned(middleTop)))
            {
                allowedAmountPosY = 1;
            }
            else if (Mathf.Approximately(allowedAmountPosY, 1f / 3))
            {
                allowedAmountPosY = 0;
            }
            if (boundingCollider.OverlapPoint(GetPointBoundsAligned(middleRight)))
            {
                allowedAmountPosX = 1;
            }
            else if (Mathf.Approximately(allowedAmountPosX, 1f / 3))
            {
                allowedAmountPosX = 0;
            }
            if (boundingCollider.OverlapPoint(GetPointBoundsAligned(middleLeft)))
            {
                allowedAmountNegX = 1;
            }
            else if (Mathf.Approximately(allowedAmountNegX, 1f / 3))
            {
                allowedAmountNegX = 0;
            }
        }
        desiredDelta.z *= 0;
        if (allowedAmountNegX == 0 && allowedAmountNegY == 0 && allowedAmountPosX ==0 && allowedAmountPosY == 0)
        {
            return desiredDelta;
        }
        if (desiredDelta.x > 0)
        {
            desiredDelta.x *= allowedAmountPosX;
        }
        else
        {
            desiredDelta.x *= allowedAmountNegX;
        }
        if (desiredDelta.y > 0)
        {
            desiredDelta.y *= allowedAmountPosY;
        }
        else
        {
            desiredDelta.y *= allowedAmountNegY;
        }
        Debug.Log(desiredDelta);
        return desiredDelta;
    }
}
