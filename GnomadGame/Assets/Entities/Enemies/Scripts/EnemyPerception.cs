using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyPerception : MonoBehaviour
{

    public EnemyStateMachine esm;

    // How far away the enemy can see things
    public float viewRadius;

    // The angle at which the enemy can see things (Vision Cone)
    [Range(0, 360)] public float viewAngle;


    // When a targetObject has been set, this will be true. And when the enemy can't see the set target object
    //  After a period of time, it will be set back to false.
    // This bool makes sure that the the logic doesn't keep setting the enemy's target object to null
    [SerializeField] private bool lookingForTargetObject = false;

    [SerializeField] private float timeSinceLastSeen = 0f;


    // The layer the enemy looks for targets on
    public LayerMask targetLayer;

    // The layer that checks if there is a wall between the target and enemy
    public LayerMask obstructionLayer;

    // The variables below are mainly used for the Unity Editor Functions
    private Collider2D[] colliders = null;
    private bool canSeeObject = false;


    // If the enemy can't see the target object
    //  Then this is set to false and will start the timeSinceLastSeen timer
    private bool canSeeTargetObject = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckFOV());
    }

    private void Update()
    {
        if (!canSeeTargetObject && lookingForTargetObject)
        {
            timeSinceLastSeen += Time.deltaTime;
            if (timeSinceLastSeen > 3f)
            {
                esm.IsAggro = false;
                esm.targetObject = null;
                timeSinceLastSeen = 0f;
                lookingForTargetObject = false;
            }
        }
    }

    private IEnumerator CheckFOV()
    {
        while (true)
        {
            
            // Doesn't execute CheckForFOVCollisions() until the 
            // time put in to WaitForSeconds passes

            // This is here so it doesn't get called every frame
            // and tank performance
            yield return new WaitForSeconds(0.3f);

            CheckForFOVCollisions();
        }
    }

    /// <summary>
    /// Checks if there are any targets within the enemy's 'viewRadius'
    /// </summary>
    private void CheckForFOVCollisions()
    {
        canSeeTargetObject = false;
        canSeeObject = false;
        // Get all collisions on the targetLayer within the enemy's viewRadius
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetLayer);

        // For debugging
        colliders = rangeCheck;

        // If there are collisions, or if the rangeCheck array isn't empty
        if (rangeCheck.Length > 0)
        {
       
            // Go through each collider that was found
            foreach (Collider2D collider in rangeCheck)
            {
                Transform target = collider.transform;
                Vector2 directionToTarget = target.position - transform.position;

                // If the object was within the enemy's viewAngle / vision cone
                if (Vector2.Angle(transform.up, directionToTarget) < viewAngle * 0.5f)
                {
                    float distanceToTarget = Vector2.Distance(target.position, transform.position);

                    // If raycasting to object location from enemy position on the ObstructionLayer,
                    // And there's no hit, then the enemy has a clear line of site of the object
                    if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionLayer))
                    {
                        // If the object is the player and not the player's hat
                        if (target.gameObject.CompareTag("Player") && target.gameObject.name != "Hat Pin")
                        {
                            
                            //Debug.Log("Player Detected");
                            esm.IsAggro = true;
                            esm.targetObject = target.gameObject;
                            lookingForTargetObject = true;

                            // For Debugging
                            canSeeObject = true;
                        }
                        if (target.gameObject == esm.targetObject)
                        {
                            canSeeTargetObject |= true;
                            esm.IsTargetOutOfSight = false;
                        }
                        
                    }
                    continue;
                }
            }

        }

        if (!canSeeTargetObject)
        {
            esm.IsTargetOutOfSight = true;
        }
        
    }

    // Functions Below Are For Unity Editor

    #if UNITY_EDITOR

    private Vector2 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, viewRadius);

        Vector3 angle1 = DirectionFromAngle(-transform.eulerAngles.z, -viewAngle * -0.5f);
        Vector3 angle2 = DirectionFromAngle(-transform.eulerAngles.z, viewAngle * -0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + angle1 * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + angle2 * viewRadius);

        //Debug.LogWarning(canSeeObject);

        if (canSeeObject)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < colliders.Length && colliders[i] != null; i++)
            {
                //Debug.LogWarning("Line Being Drawn to Object");
                Gizmos.DrawLine(transform.position, colliders[i].transform.position);
                
            }
        }

    }

    #endif

}