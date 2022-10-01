using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingTargetController : MonoBehaviour
{
    private int _layerMask;

    private void Awake()
    {
        _layerMask =~ LayerMask.GetMask("Player");
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
        {
            transform.position = hit.point;
        }
    }
}
