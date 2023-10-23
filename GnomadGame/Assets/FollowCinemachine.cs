using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gnomad.Utils;
public class FollowCinemachine : MonoBehaviour
{
    [SerializeField] GameObject cinemachineGO;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, cinemachineGO.transform.position, Utils.GetInterpolant(10));
    }
}
