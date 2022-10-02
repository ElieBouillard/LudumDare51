using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttack : MonoBehaviour
{
    private EnemyGameIdentity _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<EnemyGameIdentity>();
    }

    public void TriggerAttack()
    {
        _enemy.CastAttack();
    }
}
