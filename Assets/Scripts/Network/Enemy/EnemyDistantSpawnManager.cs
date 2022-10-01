using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyDistantSpawnManager : Singleton<EnemyDistantSpawnManager>
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private EnemyGameIdentity _enemy;
    

    public void SpawnEnemy(int id, int spawnIndex)
    {
        EnemyGameIdentity enemy = Instantiate(_enemy, _spawnPoints[spawnIndex].position, quaternion.identity);
        enemy.SetId(id);
    }
}
