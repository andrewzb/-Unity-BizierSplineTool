using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EquasionSolver
{

    public static Vector3 GetBuizierMiddlePoint(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float factor)
    {
        var localFactor = Mathf.Clamp01(factor);
        var reverseFactor = 1f - localFactor;
        return  reverseFactor * reverseFactor * reverseFactor * p0 +
                3f * reverseFactor * reverseFactor * localFactor * p1 +
                3f * reverseFactor * localFactor * localFactor * p2 +
                localFactor * localFactor * localFactor * p3;
    }

    public static Vector3 GetBuizierDerivativeFirst(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float factor)
    {
        var localFactor = Mathf.Clamp01(factor);
        var reverseFactor = 1f - localFactor;
        return 
            3f * reverseFactor * reverseFactor * (p1 - p0) + 
            6f * reverseFactor * localFactor * (p2 - p1) +
            3f * localFactor * localFactor * (p3 - p2);
    }
}
