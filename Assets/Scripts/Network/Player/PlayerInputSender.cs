using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputSender : MonoBehaviour
{
    [SerializeField] private Transform _aimTransform;
    private NetworkManager _networkManager;
    private PlayerAnimationController _playerAnimation;

    private void Awake()
    {
        _networkManager = NetworkManager.Instance;
        _playerAnimation = GetComponentInChildren<PlayerAnimationController>();
    }

    private void FixedUpdate()
    {
        _networkManager.GetClientMessages().SendInputs(transform.position, transform.GetChild(0).rotation, _playerAnimation.GetVelocityZ(), _playerAnimation.GetVelocityX(), _aimTransform.position);
    }
}
