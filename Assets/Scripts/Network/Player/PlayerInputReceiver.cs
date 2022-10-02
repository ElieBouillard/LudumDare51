using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using Gameplay.Player;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerInputReceiver : MonoBehaviour
{
    [SerializeField] private Rig _rig1;
    [SerializeField] private Rig _rig2;
    [SerializeField] private Animator _animator;

    private PlayerAnimationController _playerAnimation;

    private Vector3? _targetPos;
    private Quaternion? _targetRot;
    
    private void Awake()
    {
        _playerAnimation = GetComponentInChildren<PlayerAnimationController>();
    }

    public void SetInput(Vector3 pos, Quaternion rot, float velocityZ, float velocityX, bool isSprinting)
    {
        _targetPos = pos;
        _targetRot = rot;
        
        _playerAnimation.SetVelocityZ(velocityZ);
        _playerAnimation.SetVelocityX(velocityX);
        
        if(_animator.GetBool("Run") != isSprinting) _animator.SetBool("Run", isSprinting);
        _rig1.weight = isSprinting ? 0 :1;
        _rig2.weight = isSprinting ? 0 : 1;
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