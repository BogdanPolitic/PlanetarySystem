using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMath
{
    public static Vector4 InterpolateBetweenVector4s(Vector4 start, Vector4 end, float percentage)
    {
        return new Vector4(
            start.x + (end.x - start.x) * percentage,
            start.y + (end.y - start.y) * percentage,
            start.z + (end.z - start.z) * percentage,
            start.w + (end.w - start.w) * percentage
        );
    }
}
