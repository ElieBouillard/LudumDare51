using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Image _healthBar;
    
    private int _currHealth;

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
        
        if (_currHealth < 0)
        {
            Death();
        }
    }

    private void Death()
    {
        
    }
}   
