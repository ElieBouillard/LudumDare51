using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyServerSpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private EnemyGameIdentity _enemyPrefab;
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private int _spawnCount;

    private NetworkManager _networkManager;
    
    private List<EnemyGameIdentity> _enemies = new List<EnemyGameIdentity>();

    private float _initialTimer;
    private float _currTimer;

    private void Awake()
    {
        _networkManager = NetworkManager.Instance;
        enabled = _networkManager.GetServer().IsRunning;
    }

    private void Update()
    {
        _currTimer += Time.deltaTime;

        if (_currTimer > 10f)
        {
            Spawn();
            _currTimer = 0;
        }
    }
    
    [ContextMenu("spawn")]
    private void Spawn()
    {
        List<int> _spawnAvaible = new List<int>();

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _spawnAvaible.Add(i);
        }

        for (int i = 0; i < _spawnCount; i++)
        {
            int randIndex = Random.Range(0, _spawnAvaible.Count);

            Vector3 pos = _spawnPoints[_spawnAvaible[randIndex]].position;

            EnemyGameIdentity enemy = Instantiate(_enemyPrefab, pos, Quaternion.identity);

            
            _enemies.Add(enemy);
            
            enemy.SetId(_enemies.Count - 1);
            
            _networkManager.GetServerMessages().SendSpawnEnemy(enemy.GetId(), _spawnAvaible[randIndex]);
            
            _spawnAvaible.RemoveAt(randIndex);
        }
    }
}
