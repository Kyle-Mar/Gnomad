using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Gnomad.Utils
{
    public static class Utils
    {
        public static float GetInterpolant(float k)
        {
            //Debug.Log(k);
            // https://www.youtube.com/watch?v=YJB1QnEmlTs Timestamp: t=7:25
            return Mathf.Abs(1.0f - Mathf.Pow(k, Time.deltaTime));
        }
        public static Vector3 Sign(Vector3 vec)
        {
            return new Vector3(Mathf.Sign(vec.x), Mathf.Sign(vec.y), Mathf.Sign(vec.z));
        }

        public static Vector3Int Vector3ToVector3Int(Vector3 vec)
        {
            return new Vector3Int((int)vec.x, (int)vec.y, (int)vec.z);
        }
    }
    public static class Vector2IntExtension
    {

        public static Vector2Int Rotate(this Vector2Int v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            int tx = v.x;
            int ty = v.y;
            v.x = (int)((cos * tx) - (sin * ty));
            v.y = (int)((sin * tx) + (cos * ty));
            return v;
        }
    }
}
