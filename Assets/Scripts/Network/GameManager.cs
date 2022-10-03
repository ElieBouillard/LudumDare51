using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _localPlayerPrefab;
    [SerializeField] private GameObject _otherPlayerPrefab;
    [SerializeField] private TMP_Text[] _scoreTexts;
    [SerializeField] private int _waveNumberToRespawn;
    

    private Dictionary<ushort, int> _scores = new Dictionary<ushort, int>();
    public Transform GetSpawnPoint() => _spawnPoints[0];

    public bool IsGameOver;

    private List<PlayerIdentity> _players = new List<PlayerIdentity>();
    
    private Dictionary<ushort, int> _playersDead = new Dictionary<ushort, int>();
    
    public Dictionary<ushort, int> GetPlayersDead() => _playersDead;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        NetworkManager.Instance.GetClientMessages().SendReady();
    }

    public void SpawnPlayers()
    {
        NetworkManager networkManager = NetworkManager.Instance;

        PlayerIdentity[] playersTemp = networkManager.GetPlayers().Values.ToArray();

        foreach (var player in playersTemp)
        {
            AddPlayerInGame(player.GetId(), player.GetSteamId());
        }
    }
    
    public void AddPlayerInGame(ushort playerId, ulong steamId)
    {
        NetworkManager networkManager = NetworkManager.Instance;

        GameObject playerObject = playerId == networkManager.GetClient().Id ? _localPlayerPrefab : _otherPlayerPrefab;

        Transform spawnPoint = null;
        
        for (int i = 0; i < networkManager.GetPlayers().Values.ToArray().Length; i++)
        {
            if (networkManager.GetPlayers().Values.ToArray()[i].GetId() == playerId)
            {
                spawnPoint = _spawnPoints[i];
                break;
            }
        }

        GameObject playerTemp = Instantiate(playerObject, spawnPoint.position ,spawnPoint.rotation);
        PlayerGameIdentity playerIdentityTemp = playerTemp.GetComponent<PlayerGameIdentity>();

        if(networkManager.GetUseSteam()) playerIdentityTemp.Initialize(playerId, steamId);
        else playerIdentityTemp.Initialize(playerId, $"Player : {playerId}");

        if(playerId == networkManager.GetClient().Id) networkManager.SetLocalPlayer(playerIdentityTemp);

        networkManager.GetPlayers()[playerId] = playerIdentityTemp;
        
        _players.Add(playerIdentityTemp);
        
        _scores.Add(playerIdentityTemp.GetId(), 0);
    }

    public void RemovePlayerFromGame(ushort playerId)
    {
        NetworkManager networkManager = NetworkManager.Instance;
        foreach (var player in networkManager.GetPlayers())
        {
            if (player.Key == playerId)
            {
                Destroy(player.Value.gameObject);
                networkManager.GetPlayers().Remove(player.Key);
                return;
            }
        }
    }
    
    public void ClearPlayerInGame()
    {
        NetworkManager networkManager = NetworkManager.Instance;
        
        networkManager.SetLocalPlayer(null);
        
        foreach (var player in networkManager.GetPlayers())
        {
            Destroy(player.Value.gameObject);
        }
            
        networkManager.GetPlayers().Clear();
    }

    public void AddScore(ushort id)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].GetId() == id)
            {
                _scores[id]++;

                _scoreTexts[i].text = $"{_players[i].gameObject.name} : {_scores[id]}";
            }
        }
    }

    public void OnGameOver(int waveIndex)
    {
        IsGameOver = true;
        
        GameOverPanel.Instance.EnableGameOverPanel(waveIndex);

        List<int> scoresToSend = new List<int>();

        foreach (var score in _scores)
        {
            scoresToSend.Add(score.Value);
        }
        
        scoresToSend.Sort();
        scoresToSend.Reverse();

        for (int i = 0; i < scoresToSend.Count; i++)
        {
            foreach (var score in _scores)
            {
                if (scoresToSend[i] == score.Value)
                {
                    GameOverPanel.Instance.AddScore(score.Key, score.Value);
                    _scores.Remove(score.Key);
                    break;
                }
            }
        }
    }
    
    public void OnPlayerDead(ushort playerId)
    {
        _playersDead.Add(playerId, EnemySpawnManager.Instance.GetCurrWave());

        if (_playersDead.Count >= NetworkManager.Instance.GetPlayers().Count)
        {
            NetworkManager.Instance.GetServerMessages().SendGameOver(EnemySpawnManager.Instance.GetCurrWave());
        }
    }

    public void CheckForPlayerRespawn(int waveIndex)
    {
        List<ushort> playerToRespawn = new List<ushort>();

        foreach (var player in _playersDead)
        {
            if (player.Value + _waveNumberToRespawn == waveIndex)
            {
                playerToRespawn.Add(player.Key);
                NetworkManager.Instance.GetServerMessages().SendOnClientRespawn(player.Key);
            }
        }

        for (int i = 0; i < playerToRespawn.Count; i++)
        {
            _playersDead.Remove(playerToRespawn[i]);
        }
    }
}
