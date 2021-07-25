using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BizierSplineSection
{
    public Vector3 mainPoint;
    public Vector3 nextSupportPoint;
    public Vector3 prevSupportPoint;

    public BizierSplineSection(Vector3 mainPoint, Vector3 nextSupportPoint, Vector3 prevSupportPoint)
    {
        this.mainPoint = mainPoint;
        this.nextSupportPoint = nextSupportPoint;
        this.prevSupportPoint = prevSupportPoint;
    }
}
