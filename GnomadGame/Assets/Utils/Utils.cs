using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float GetInterpolant(float k)
    {
        // https://www.youtube.com/watch?v=YJB1QnEmlTs Timestamp: t=7:25
        return Mathf.Abs(1.0f - Mathf.Pow(k, Time.deltaTime));
    }
    public static Vector3 Sign(Vector3 vec)
    {
        return new Vector3(Mathf.Sign(vec.x), Mathf.Sign(vec.y), Mathf.Sign(vec.z));
    }
}
