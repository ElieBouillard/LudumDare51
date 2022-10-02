using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingTargetController : MonoBehaviour
{
    [SerializeField] private bool _islocal;
    [SerializeField] private LayerMask _layerMask;

    private Vector3 _targetPos;

    public void SetTargetPos(Vector3 pos) => _targetPos = pos;

    private void Update()
    {
        if (_islocal)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
            {
                _targetPos = hit.point;
            }
        }

        transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * 100f);
    }
}