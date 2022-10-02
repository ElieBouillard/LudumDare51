using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerIdentity playerIdentity))
        {
            if (playerIdentity.GetId() != NetworkManager.Instance.GetLocalPlayer().GetId()) return;
            
            Vector3 pos = playerIdentity.transform.position;
            pos.y = 1;
            playerIdentity.transform.position = pos;
            playerIdentity.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
