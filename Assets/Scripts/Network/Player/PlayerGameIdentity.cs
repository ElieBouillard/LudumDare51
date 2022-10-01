using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class PlayerGameIdentity : PlayerIdentity
{
    public PlayerInputReceiver PlayerInputReceiver;
    public AimingTargetController PlayerAimController;
    public PlayerDistantFiringController PlayerDistantFiringController;

    public override void Initialize(ushort id, string newName)
    {
        base.Initialize(id, newName);

        if (id == NetworkManager.Instance.GetClient().Id)
        {
            CameraController.Instance.SetTarget(transform);
        }
    }
}   
