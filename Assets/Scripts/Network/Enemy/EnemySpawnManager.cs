using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawnManager : Singleton<EnemySpawnManager>
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private EnemyGameIdentity enemyPrefab;
    [SerializeField] private int _spawnCount;
    [SerializeField] private int _waveToAdd;


    private NetworkManager _networkManager;
    
    private List<EnemyGameIdentity> _enemies = new List<EnemyGameIdentity>();

    private float _initialTimer;
    private float _currTimer;

    private int _currWave = 0;

    public int GetCurrWave() => _currWave;
    public List<EnemyGameIdentity> GetEnemies() => _enemies;

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
        if (GameManager.Instance.IsGameOver) return;
        
        _currWave++;
        _networkManager.GetServerMessages().SendChangeWave(_currWave);
        
        GameManager.Instance.CheckForPlayerRespawn(_currWave);
        
        List<int> _spawnAvaible = new List<int>();

        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _spawnAvaible.Add(i);
        }

        for (int i = 0; i < _spawnCount; i++)
        {
            int randIndex = Random.Range(0, _spawnAvaible.Count);

            Vector3 pos = _spawnPoints[_spawnAvaible[randIndex]].position;

            EnemyGameIdentity enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            
            _enemies.Add(enemy);
            
            enemy.SetId(Random.Range(0,999999999));
            
            _networkManager.GetServerMessages().SendSpawnEnemy(enemy.GetId(), _spawnAvaible[randIndex]);
            
            _spawnAvaible.RemoveAt(randIndex);
        }
    }

    public void ServerSpawnEnemy(int id, int spawnIndex)
    {
        EnemyGameIdentity enemy = Instantiate(enemyPrefab, _spawnPoints[spawnIndex].position, Quaternion.identity);
        enemy.SetId(id);
        _enemies.Add(enemy);
    }
}