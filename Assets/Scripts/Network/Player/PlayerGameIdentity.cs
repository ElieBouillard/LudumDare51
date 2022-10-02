using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using DG.Tweening;
using Steamworks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerGameIdentity : PlayerIdentity
{
    public PlayerInputReceiver PlayerInputReceiver;
    public AimingTargetController PlayerAimController;
    public PlayerDistantFiringController PlayerDistantFiringController;

    [SerializeField] private int _initialHealth;

    [SerializeField] private LineRenderer _line;
    private PlayerLocalFiringController _playerFire;
    private AdvancedWalkerController _movementController;
    private Rigidbody _rb;

    private int _currHealth;

    private bool _isSpectate;
    
    private void Awake()
    {
        _playerFire = GetComponent<PlayerLocalFiringController>();
        _movementController = GetComponent<AdvancedWalkerController>();
        _rb = GetComponent<Rigidbody>();
    }

    public override void Initialize(ushort id, string newName)
    {
        base.Initialize(id, newName);

        if (id == NetworkManager.Instance.GetClient().Id)
        {
            CameraController.Instance.SetTarget(transform);
        }

        _currHealth = _initialHealth;
    }

    public void TakeDamage(int damage)
    {
        _currHealth -= damage;

        if(IsLocalPlayer())
            HealthBarManager.Instance.SetHealthBarValue((float)_currHealth /(float)_initialHealth);
        
        if (_currHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        LocalEnable(false);
    }
    
    public void Revive()
    {
        LocalEnable(true);
    }
    
    private void LocalEnable(bool value)
    {
        if (value)
        {
            _currHealth = _initialHealth;
            HealthBarManager.Instance.SetHealthBarValue((float)_currHealth /(float)_initialHealth);
            
            CameraController.Instance.SetTarget(transform);
            transform.position = new Vector3(-1.5f,0,-20f);
        }
        else
        {
            foreach (var player in NetworkManager.Instance.GetPlayers())
            {
                if (player.Value.transform.position.y < 500)
                {
                    if (player.Key != GetId())
                    {
                        CameraController.Instance.SetTarget(player.Value.transform);
                    }
                }
            }
            
            transform.position = new Vector3(0, 1000f, 0);
            
            NetworkManager.Instance.GetClientMessages().SendOnDead();
        }
        
        _isSpectate = !value;
        _movementController.enabled = value;
        _rb.velocity = Vector3.zero;
        _playerFire.enabled = value;
        _line.enabled = value;
    }
}   
