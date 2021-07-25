using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BizierCubicSpline : MonoBehaviour
{
    [Header("Points Prefab")]
    [SerializeField] private GameObject _mainPointPrefab = null;
    [SerializeField] private GameObject _supportPointPrefab = null;
    [Space]

    [Header("Containers")]
    [SerializeField] private Transform _mainContainer = null;
    [SerializeField] private Transform _supportContainer = null;
    [Space]

    [Header("Containers")]
    [SerializeField] private List<Transform> _mainPointsList = null;
    [SerializeField] private List<Transform> _supportPointsList = null;
    [Space]

    [Header("Settings")]
    [SerializeField] private bool _showPath = false;
    [SerializeField] private bool _isClosed = false;
    [SerializeField] private float _whiskerLength = 1f;
    [SerializeField] private bool _isCloseSpline = false;
    [Range(5, 100)]
    [SerializeField] private int _sectionsCount = 5;

    private List<BizierSplineSection> _splineDescriptor = null;

    #region  LIFE_CYCLE

    private void OnDrawGizmos()
    {

        if (_splineDescriptor != null && _splineDescriptor.Count > 0)
            DrawSplineByData();
        else
            DrawSplineByObjects();
    }

    #endregion

    #region UTIL_METHOD

    private void DrawSplineByObjects()
    {
        if (_mainPointPrefab == null || _mainPointsList.Count <= 2)
            return;
        if (_supportPointsList != null && _supportPointsList.Count >= 4)
        {
            var count = _isClosed ? _mainPointsList.Count : _mainPointsList.Count - 1;
            for (int i = 0; i < count; i++)
            {
                var currentPoint = _mainPointsList[i].position;
                var currentPointNextSupport = _supportPointsList[i * 2 + 1].position;

                var nextPoint = Vector3.zero;
                var nextPointSupport = Vector3.zero;
                var isLast = i == count - 1;

                if (_isClosed)
                {
                    nextPoint = _mainPointsList[isLast ? 0 : i + 1].position;
                    nextPointSupport = _supportPointsList[isLast ? 0 : (i + 1) * 2].position;
                }
                else
                {
                    nextPoint = _mainPointsList[i + 1].position;
                    nextPointSupport = _supportPointsList[(i + 1) * 2].position;
                }

                var startPos = currentPoint;
                for (int j = 0; j <= _sectionsCount; j++)
                {
                    var factor = (float)j / _sectionsCount;
                    var endPos = EquasionSolver.GetBuizierMiddlePoint(
                    currentPoint, currentPointNextSupport, nextPointSupport, nextPoint, factor);
                    Gizmos.DrawLine(startPos, endPos);
                    startPos = endPos;
                }
            }
        }
        else
        {
            for (int i = 0; i < _mainPointsList.Count - 1; i++)
            {
                Gizmos.DrawLine(_mainPointsList[i].position, _mainPointsList[i + 1].position);
            }
        }
    }

    private void DrawSplineByData()
    {
        if (_splineDescriptor == null || _splineDescriptor.Count <= 2)
            return;
        var count = _isClosed ? _splineDescriptor.Count : _splineDescriptor.Count - 1;
        for (int i = 0; i < count; i++)
        {
            var currentPoint = _splineDescriptor[i].mainPoint;
            var currentPointNextSupport = _splineDescriptor[i].nextSupportPoint;

            var nextPoint = Vector3.zero;
            var nextPointSupport = Vector3.zero;

            var isLast = i == count - 1;

            if (_isClosed)
            {
                nextPoint = _splineDescriptor[isLast ? 0 : i + 1].mainPoint;
                nextPointSupport = _splineDescriptor[isLast ? 0 : i + 1].prevSupportPoint;
            }
            else
            {
                nextPoint = _splineDescriptor[i + 1].mainPoint;
                nextPointSupport = _splineDescriptor[i + 1].prevSupportPoint;
            }

            var startPos = currentPoint;
            for (int j = 0; j <= _sectionsCount; j++)
            {
                var factor = (float)j / _sectionsCount;
                var endPos = EquasionSolver.GetBuizierMiddlePoint(
                currentPoint, currentPointNextSupport, nextPointSupport, nextPoint, factor);
                Gizmos.DrawLine(startPos, endPos);
                startPos = endPos;
            }
        }
    }

    #endregion

    #region EDITOR_METHOD
    [ContextMenu("GenerateSplineData")]
    private void GenerateSplineData()
    {
        if(_splineDescriptor != null)
            _splineDescriptor.Clear();
        else
            _splineDescriptor = new List<BizierSplineSection>();
        if (_mainPointPrefab == null || _mainPointsList.Count <= 2
          || _supportPointsList == null || _supportPointsList.Count <= 4)
          return;
        var count = _mainPointsList.Count;
        for (int i = 0; i < count; i++)
        {
            _splineDescriptor.Add(
                new BizierSplineSection(
                _mainPointsList[i].position,
                _supportPointsList[i * 2 + 1].position,
                _supportPointsList[i * 2].position)
            );
        }
    }

    [ContextMenu("ClearSplineData")]
    private void ClearSplineData()
    {
        if (_splineDescriptor != null && _splineDescriptor.Count > 0)
            _splineDescriptor.Clear();
        _splineDescriptor = new List<BizierSplineSection>();
    }

    [ContextMenu("Generate Support Points")]
    private void GenerateSupportPoints()
    {
        if (_supportPointsList != null && _supportPointsList.Count > 0)
            foreach (var item in _supportPointsList)
                Destroy(item.gameObject);
        _supportPointsList = new List<Transform>();

        if (_mainPointsList == null && _mainPointsList.Count < 2)
        {
            // putt error message in UI editor
            throw new Exception("At least 2 main points in spline");
            //return;
        }

        var posList = new List<Vector3>();
        var count = _mainPointsList.Count;
        for (int i = 0; i < count; i++)
        {
            var mainPointPos = _mainPointsList[i].position;
            var isLast = i == count - 1;
            var dirVector = Vector3.up;

            dirVector = (_mainPointsList[isLast ? 0 : i + 1].position - _mainPointsList[i].position).normalized;

            var upVector = Vector3.up;
            // perpendicular 
            // c1 c2 c3
            // a1 a2 a3 dirVector
            // b1 b2 b3 upVector
            // c1 (a2b3 - b2a3) + c2(a3b1 - b3a1) + c3(a1b2 - b1a2)
  
            var next = mainPointPos + dirVector * _whiskerLength;
            var prev = mainPointPos - dirVector * _whiskerLength;
            posList.Add(prev);
            posList.Add(next);
        }

        foreach (var pos in posList)
        {
            var supportPoint = Instantiate(_supportPointPrefab, pos, Quaternion.identity);
            supportPoint.transform.SetParent(_supportContainer);
            _supportPointsList.Add(supportPoint.transform);
        }
    }

    [ContextMenu("CleanSupportPoints")]
    private void CleanSupportPoints()
    {
        //TODO do not work in not play mode
        if (_supportPointsList != null && _supportPointsList.Count > 0)
            foreach (var item in _supportPointsList)
                Destroy(item.gameObject);
        _supportPointsList = new List<Transform>();
    }

    [ContextMenu("CleanMainPoints")]
    private void CleanMainPoints()
    {
        //TODO do not work in not play mode
        if (_mainPointsList != null && _mainPointsList.Count > 0)
            foreach (var item in _mainPointsList)
                Destroy(item.gameObject);
        _mainPointsList = new List<Transform>();
    }
    #endregion
}
