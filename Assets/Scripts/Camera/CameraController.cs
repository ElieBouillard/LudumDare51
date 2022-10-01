using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offSet;
    [SerializeField] private float _smoothForce;

    public void SetTarget(Transform target) => _target = target;
    
    private void FixedUpdate()
    {
        if (_target == null) return;
        transform.position = Vector3.Lerp(transform.position, _target.position + _offSet, Time.deltaTime * _smoothForce);
    }
}