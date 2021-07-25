using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEngine.Debug;

[ExecuteAlways]
public class BiezieCurve : MonoBehaviour
{
    [SerializeField] private Transform _p0 = null;
    [SerializeField] private Transform _p1 = null;
    [SerializeField] private Transform _p2 = null;
    [SerializeField] private Transform _p3 = null;

    [Header("Drawing")]
    [Range(1, 100)]
    [SerializeField] private ushort _sections = 20;

    private void OnDrawGizmos()
    {
       var startPos = _p0.position; 
       for (int i = 0; i <= _sections; i++)
       {
            var factor = (float)i / _sections;
            Debug.Log(factor);
            var endPos = EquasionSolver.GetBuizierMiddlePoint(
                _p0.position, _p1.position, _p2.position, _p3.position, factor);
            Gizmos.DrawLine(startPos, endPos);
            startPos = endPos;
       }
    }



}
