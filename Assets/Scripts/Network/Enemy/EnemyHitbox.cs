using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private List<PlayerGameIdentity> _playersHit = new List<PlayerGameIdentity>();

    public List<PlayerGameIdentity> GetPlayersHit() => _playersHit;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerGameIdentity playerIdentity))
        {
            if (!_playersHit.Contains(playerIdentity))
            {
                _playersHit.Add(playerIdentity);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlayerGameIdentity playerIdentity))
        {
            if (_playersHit.Contains(playerIdentity))
            {
                _playersHit.Remove(playerIdentity);
            }
        }
    }
}
