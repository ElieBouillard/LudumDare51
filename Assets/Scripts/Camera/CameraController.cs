using System;
using System.Collections;
using System.Collections.Generic;
using CameraShake;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offSet;
    [SerializeField] private float _smoothForce;

    private void Start()
    {
        CameraShaker.Instance.StrengthMultiplier = 1.5f;
    }

    public void SetTarget(Transform target) => _target = target;
    
    private void FixedUpdate()
    {
        if (_target == null) return;
        transform.position = Vector3.Lerp(transform.position, _target.position + _offSet, Time.deltaTime * _smoothForce);
    }
}