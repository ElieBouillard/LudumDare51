using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerInputReceiver : MonoBehaviour
{
    private PlayerAnimationController _playerAnimation;

    private Vector3? _targetPos;
    private Quaternion? _targetRot;
    
    private void Awake()
    {
        _playerAnimation = GetComponentInChildren<PlayerAnimationController>();
    }

    public void SetInput(Vector3 pos, Quaternion rot, float velocityZ, float velocityX)
    {
        _targetPos = pos;
        _targetRot = rot;
        
        _playerAnimation.SetVelocityZ(velocityZ);
        _playerAnimation.SetVelocityX(velocityX);
    }

    private void Update()
    {
        if (_targetPos != null)
        {
            transform.position = Vector3.LerpUnclamped(transform.position, _targetPos.Value, Time.deltaTime * 10f);
        }

        if (_targetRot != null)
        {
            transform.GetChild(0).rotation = Quaternion.LerpUnclamped(transform.GetChild(0).rotation, _targetRot.Value, Time.deltaTime * 10f);
        }
    }
}