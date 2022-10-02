using RiptideNetworking;
using UnityEngine;

public class ClientMessages : MonoBehaviour
{
    internal enum MessagesId : ushort
    {
        Ping = 1,
        ClientConnected,
        StartGame,
        Ready,
        Inputs,
        Shoot,
        ShootEnemy,
        Dead
    }

    private static NetworkManager _networkManager;

    private void Awake()
    {
        _networkManager = NetworkManager.Instance;
    }

    #region Send

    public void SendPing(float time)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.Ping);
        message.AddFloat(time);
        _networkManager.GetClient().Send(message);
    }
    
    public void SendClientConnected(ulong steamId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.ClientConnected);
        message.AddULong(steamId);
        _networkManager.GetClient().Send(message);
    }
    
    public void SendStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.StartGame);
        _networkManager.GetClient().Send(message);
    }

    public void SendReady()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.Ready);
        _networkManager.GetClient().Send(message);
    }

    public void SendInputs(Vector3 pos, Quaternion rot, float velocityZ, float velocityX, Vector3 aimPos, bool isSprinting) 
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.Inputs);
        message.AddVector3(pos);
        message.AddQuaternion(rot);
        message.AddFloat(velocityZ);
        message.AddFloat(velocityX);
        message.AddVector3(aimPos);
        message.AddBool(isSprinting);
        _networkManager.GetClient().Send(message);
    }

    public void SendShoot(Vector3 pos, Vector3 dir)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.Shoot);
        message.AddVector3(pos);
        message.AddVector3(dir);
        _networkManager.GetClient().Send(message);
    }
    
    public void SendShootEnemy(int id, Vector3 pos, Vector3 dir)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.ShootEnemy);
        message.AddInt(id);
        message.AddVector3(pos);
        message.AddVector3(dir);
        _networkManager.GetClient().Send(message);
    }

    public void SendOnDead()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.Dead);
        _networkManager.GetClient().Send(message);
    }
    #endregion

    #region Received

    [MessageHandler((ushort)ServerMessages.MessagesId.Ping)]
    private static void OnServerPing(Message message)
    {
        if (!PingSystem.Instance) return;
        PingSystem.Instance.ReceivePing(message.GetFloat());
    }
    
    [MessageHandler((ushort) ServerMessages.MessagesId.PlayerConnectedToLobby)]
    private static void OnClientConnectedToLobby(Message message)
    {
        LobbyManager.Instance.AddPlayerToLobby(message.GetUShort(), message.GetULong());
    } 
    
    [MessageHandler((ushort) ServerMessages.MessagesId.PlayerDisconnected)]
    private static void OnClientDisconnected(Message message)
    {
        ushort id = message.GetUShort();
        
        switch (_networkManager.GetGameState())
        {
            case GameState.Lobby:
                LobbyManager.Instance.RemovePlayerFromLobby(id);
                break;
            
            case GameState.Gameplay:
                GameManager.Instance.RemovePlayerFromGame(id);
                break;
        }
        
    } 
    
    [MessageHandler((ushort) ServerMessages.MessagesId.StartGame)]
    private static void OnServerStartGame(Message message)
    {
        _networkManager.OnServerStartGame();
    }

    [MessageHandler((ushort) ServerMessages.MessagesId.InitializeGameplay)]
    private static void OnServerInitializeClient(Message message)
    {
        GameManager.Instance.SpawnPlayers();
    }
    
    [MessageHandler((ushort) ServerMessages.MessagesId.PlayerInputs)]
    private static void OnServerClientInputs(Message message)
    {
        ushort id = message.GetUShort();
        Vector3 pos = message.GetVector3();
        Quaternion rot = message.GetQuaternion();
        float velocityZ = message.GetFloat();
        float velocityX = message.GetFloat();
        Vector3 aimPos = message.GetVector3();
        bool isSprinting = message.GetBool();

        foreach (var player in _networkManager.GetPlayers())
        {
            if (player.Value == null) return;

            if (player.Key == id)
            {
                PlayerGameIdentity playerIdentity = (PlayerGameIdentity)player.Value;
                playerIdentity.PlayerInputReceiver.SetInput(pos, rot, velocityZ, velocityX, isSprinting);
                playerIdentity.PlayerAimController.SetTargetPos(aimPos);
            }
        }
    }

    [MessageHandler((ushort)ServerMessages.MessagesId.ClientShoot)]
    private static void OnServerClientShoot(Message message)
    {
        ushort id = message.GetUShort();
        Vector3 pos = message.GetVector3();
        Vector3 dir = message.GetVector3();

        foreach (var player in _networkManager.GetPlayers())
        {
            if (player.Value == null) return;

            if (id == player.Key)
            {
                PlayerGameIdentity playerGameIdentity = (PlayerGameIdentity)player.Value;
                playerGameIdentity.PlayerDistantFiringController.ShootFx(pos, dir);
            }
        }
    }

    [MessageHandler((ushort)ServerMessages.MessagesId.ClientShootEnemy)]
    private static void OnServerClientShootEnemy(Message message)
    {
        ushort id = message.GetUShort();
        int enemyId = message.GetInt();
        Vector3 pos = message.GetVector3();
        Vector3 dir = message.GetVector3();

        foreach (var player in _networkManager.GetPlayers())
        {
            if (player.Value == null) return;

            if (id == player.Key)
            {
                PlayerGameIdentity playerGameIdentity = (PlayerGameIdentity)player.Value;
                playerGameIdentity.PlayerDistantFiringController.ShootEnemy(enemyId,pos, dir);
            }
        }
    }

    [MessageHandler((ushort)ServerMessages.MessagesId.SpawnEnemy)]
    private static void OnServerSpawnEnemy(Message message)
    {
        int id = message.GetInt();
        int spawnIndex = message.GetInt();
        
        EnemySpawnManager.Instance.ServerSpawnEnemy(id, spawnIndex);
    }

    [MessageHandler((ushort)ServerMessages.MessagesId.EnemyState)]
    private static void OnServerEnemyState(Message message)
    {
        int id = message.GetInt();
        Vector3 pos = message.GetVector3();
        Quaternion rot = message.GetQuaternion();
        bool isRunning = message.GetBool();
        float attack = message.GetFloat();

        foreach (var enemy in EnemySpawnManager.Instance.GetEnemies())
        {
            if (enemy.GetId() == id)
            {
                enemy.ReceivedState(pos, rot, isRunning, attack);
                return;
            }
        }
    }

    [MessageHandler((ushort) ServerMessages.MessagesId.ChangeWave)]
    private static void OnServerChangeWave(Message message)
    {
        WaveDisplay.Instance.ChangeWave(message.GetInt());
    }

    [MessageHandler((ushort) ServerMessages.MessagesId.RespawnPlayer)]
    private static void OnServerRespawn(Message message)
    {
        PlayerGameIdentity playerGameIdentity = (PlayerGameIdentity) _networkManager.GetLocalPlayer();
        playerGameIdentity.Revive();
    }
    #endregion
}