using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemySpawnManager : Singleton<EnemySpawnManager>
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private EnemyServerGameIdentity enemyServerPrefab;
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private int _spawnCount;

    private NetworkManager _networkManager;
    
    private List<EnemyServerGameIdentity> _enemies = new List<EnemyServerGameIdentity>();

    private float _initialTimer;
    private float _currTimer;

    public List<EnemyServerGameIdentity> GetEnemies() => _enemies;

    protected override void Awake()
    {
        base.Awake();
        
        _networkManager = NetworkManager.Instance;
    }

    private void Update()
    {
        if (!_networkManager.GetServer().IsRunning) return;
        
        _currTimer += Time.deltaTime;

        if (_currTimer > 10f)
        {
            Spawn();
            _currTimer = 0;
        }
    }
    
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

            EnemyServerGameIdentity enemyServer = Instantiate(enemyServerPrefab, pos, Quaternion.identity);
            
            _enemies.Add(enemyServer);
            
            enemyServer.SetId(Random.Range(0,999999999));
            
            _networkManager.GetServerMessages().SendSpawnEnemy(enemyServer.GetId(), _spawnAvaible[randIndex]);
            
            _spawnAvaible.RemoveAt(randIndex);
        }
    }

    public void ServerSpawnEnemy(int id, int spawnIndex)
    {
        EnemyServerGameIdentity enemyServer = Instantiate(enemyServerPrefab, _spawnPoints[spawnIndex].position, Quaternion.identity);
        enemyServer.SetId(id);
        _enemies.Add(enemyServer);
    }
}