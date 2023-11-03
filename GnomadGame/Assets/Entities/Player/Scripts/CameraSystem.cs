using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Gnomad.Utils;

public static class CameraSystemEvent
{
    public delegate void OnShake();
    public static OnShake onShake;
}

public class CameraSystem : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] bool doRenderSpheres;
    [Header("Player Info")]
    [SerializeField] Transform playerTransform;
    [SerializeField] BoxCollider2D playerCollider;
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] PlayerStateMachine psm;
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
    [SerializeField] float minFallingVelocity;
    [SerializeField] float fallingAnticipationMultiplier;
    [SerializeField] float offsetY;
    [SerializeField] float ledgeOffsetY;
    [Header("Current Collision Info")]
    [SerializeField] CompositeCollider2D boundingCollider;
    [SerializeField] float allowedAmountPosX;
    [SerializeField] float allowedAmountPosY;
    [SerializeField] float allowedAmountNegX;
    [SerializeField] float allowedAmountNegY;

    Vector3 bottomLeft;
    Vector3 bottomRight;
    Vector3 topLeft;
    Vector3 topRight;
    Vector3 middleBottom;
    Vector3 middleTop;
    Vector3 middleLeft;
    Vector3 middleRight;

    Vector3 playerPosViewport;

    float curOffset;


    LayerMask groundLayerMask;
    public float shakeTimer = 0f;

    void Start()
    {

        Assert.IsNotNull(playerTransform);
        Assert.IsNotNull(playerRB);
        Assert.IsNotNull(playerCollider);
        Assert.IsNotNull(psm);
        CameraSystemEvent.onShake += Shake;
        desiredPosition = GetCameraPosFromPlayerPos();
        currentAnticipation = GetAnticipationVector();
        originalPosition = transform.position;
        CalculateCollisionPoints();

        groundLayerMask = LayerMask.GetMask("Ground");
        curOffset = offsetY;
        
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
        playerPosViewport = GetComponent<Camera>().WorldToViewportPoint(playerTransform.position);

        var playerPos = playerTransform.position;
        
        Vector3 extents = playerCollider.bounds.extents;
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(playerPos.x + extents.x, playerPos.y - extents.y),
            Vector2.down,
            3f,
            groundLayerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(playerPos.x - extents.x, playerPos.y - extents.y),
            Vector2.down,
            3f,
            groundLayerMask);

        if (hitRight && hitLeft && psm.CurrentState != psm.WallSlideState && psm.CurrentState != psm.WallJumpState)
        {
            curOffset = offsetY;
        }
        else
        {
            curOffset = ledgeOffsetY;
        }
        
        Debug.DrawRay(new Vector3(playerPos.x+playerCollider.bounds.extents.x, playerPos.y - playerCollider.bounds.extents.y, playerPos.z), Vector3.down, Color.blue, 10f);
        var nextAnticipation = GetAnticipationVector();
        currentAnticipation = new Vector3(
                                Mathf.Lerp(currentAnticipation.x, nextAnticipation.x, Utils.GetInterpolant(smoothingFactorAnticipationX)),
                                Mathf.Lerp(currentAnticipation.y, nextAnticipation.y, Utils.GetInterpolant(smoothingFactorAnticipationY)),
                                currentAnticipation.z
                                );
        desiredPosition = GetCameraPosFromPlayerPos() + currentAnticipation;
        desiredPosition.z = originalPosition.z;
        //Debug.Log(playerPosViewport.y);
        
        if(shakeTimer >= 0f)
        {
            shakeTimer -= Time.fixedDeltaTime;
            desiredPosition += new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0)*2.5f;
        }
        
        
        desiredDelta = Vector3.Lerp(transform.position, desiredPosition, Utils.GetInterpolant(smoothingFactor + Mathf.Abs(40 * 0.5f-playerPosViewport.y))) - transform.position;
        
        if (boundingCollider)
        {
            transform.position += GetAllowedDelta(desiredDelta);
        }
        else
        {
            transform.position += desiredDelta;
        }
    }

    public void Shake()
    {
        shakeTimer = 0.06f;
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
        
        if(psm.CurrentState == psm.GroundedState || psm.CurrentState == psm.WallSlideState || psm.CurrentState == psm.WallJumpState)
        {
            originalPosition = new(playerTransform.position.x, playerTransform.position.y, transform.position.z);
            return;
        }
        if (playerRB.velocity.y < -15.5f)
        {
            originalPosition = new(playerTransform.position.x, playerTransform.position.y, transform.position.z);
            return;
        }
        if(playerPosViewport.y < yDeadzone.x)
        {
            originalPosition = new(playerTransform.position.x, playerTransform.position.y, transform.position.z);
            return;
        }
        if (playerPosViewport.y < yDeadzone.y)
        {
           originalPosition = new(playerTransform.position.x, playerTransform.position.y, transform.position.z);
           return;
        }
    }

    Vector3 GetCameraPosFromPlayerPos()
    {
        return new Vector3(playerTransform.position.x, curOffset + originalPosition.y, transform.position.z);
    }

    Vector3 GetAnticipationVector()
    {
        var anticipationVector = new Vector3(playerRB.velocity.x, playerRB.velocity.y, 0);
        if (playerRB.velocity.magnitude > 1)
        {
            anticipationVector.Normalize();
            if (playerRB.velocity.y < minFallingVelocity)
            {
                if (psm.CurrentState == psm.GroundPoundState)
                {
                    anticipationVector = Vector3.Scale(anticipationVector, new Vector3(horizontalAnticipation, verticalAnticipation * fallingAnticipationMultiplier, 0));
                }
                else
                {
                    anticipationVector = Vector3.Scale(anticipationVector, new Vector3(horizontalAnticipation, verticalAnticipation * fallingAnticipationMultiplier, 0));
                }
            }
            else
            {
                anticipationVector = Vector3.Scale(anticipationVector, new Vector3(horizontalAnticipation, 0, 0));

            }
        }
        //Debug.Log(anticipationVector);
        return anticipationVector;
    }
    void CalculateCollisionPoints()
    {
        if (!boundingCollider) { return; }
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
        if (!boundingCollider.OverlapPoint(GetPointBoundsAligned(playerTransform.position)))
        {
            allowedAmountPosX = 1;
            allowedAmountPosY = 1;
            allowedAmountNegX = 1;
            allowedAmountNegY = 1;
        }
        desiredDelta.z *= 0;
        if (allowedAmountNegX == 0 && allowedAmountNegY == 0 && allowedAmountPosX ==0 && allowedAmountPosY == 0)
        {
            Debug.LogWarning("[CameraSystem.cs] We lost the player. Are you missing something? We might be going too fast.");
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
        //Debug.Log(desiredDelta);
        return desiredDelta;
    }
}
