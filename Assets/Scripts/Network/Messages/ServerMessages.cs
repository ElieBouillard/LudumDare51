using RiptideNetworking;
using UnityEngine;

public class ServerMessages : MonoBehaviour
{
    internal enum MessagesId : ushort
    {
        Ping = 1,
        PlayerConnectedToLobby,
        PlayerDisconnected,
        StartGame,
        InitializeGameplay,
        PlayerInputs,
        ClientShoot,
        ClientShootEnemy,
        SpawnEnemy,
        EnemyState,
        ChangeWave,
        PlayerDead,
        PlayerRespawn,
        GameOver
    }

    private static NetworkManager _networkManager;

    private void Awake()
    {
        _networkManager = NetworkManager.Instance;
    }
    
    #region Send
    private static void SendPlayerPing(ushort id, float time)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.Ping);
        message.AddFloat(time);
        _networkManager.GetServer().Send(message, id);
    }
    
    public static void SendPlayerConnectedToLobby(ushort newPlayerId, ulong steamId)
    {
        foreach (var player in NetworkManager.Instance.GetPlayers())
        {
            Message message1 = Message.Create(MessageSendMode.reliable, MessagesId.PlayerConnectedToLobby);
            message1.AddUShort(player.Value.GetId());
            message1.AddULong(player.Value.GetSteamId());
            _networkManager.GetServer().Send(message1, newPlayerId);
        }
        
        Message message2 = Message.Create(MessageSendMode.reliable, MessagesId.PlayerConnectedToLobby);
        message2.AddUShort(newPlayerId);
        message2.AddULong(steamId);
        _networkManager.GetServer().SendToAll(message2);
    }
    
    public void SendPlayerDisconnected(ushort playerId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.PlayerDisconnected);
        message.AddUShort(playerId);
        _networkManager.GetServer().SendToAll(message, playerId);
    }

    private static void SendHostStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.StartGame);
        _networkManager.GetServer().SendToAll(message);
    }

    private static void SendInitializeClient(ushort id)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.InitializeGameplay);
        _networkManager.GetServer().Send(message, id);
    }

    private static void SendClientInputs(ushort id, Vector3 pos, Quaternion rot, float velocityZ, float velocityX, Vector3 aimPos, bool isSprinting)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.PlayerInputs);
        message.AddUShort(id);
        message.AddVector3(pos);
        message.AddQuaternion(rot);
        message.AddFloat(velocityZ);
        message.AddFloat(velocityX);
        message.AddVector3(aimPos);
        message.AddBool(isSprinting);
        _networkManager.GetServer().SendToAll(message, id);
    }

    private static void SendClientShoot(ushort id, Vector3 pos, Vector3 dir)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.ClientShoot);
        message.AddUShort(id);
        message.AddVector3(pos);
        message.AddVector3(dir);
        _networkManager.GetServer().SendToAll(message, id);
    }

    private static void SendClientShootEnemy(ushort id, int enemyId, Vector3 pos, Vector3 dir)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.ClientShootEnemy);
        message.AddUShort(id);
        message.AddInt(enemyId);
        message.AddVector3(pos);
        message.AddVector3(dir);
        _networkManager.GetServer().SendToAll(message, id);
    }
    
    public void SendSpawnEnemy(int id, int spawnIndex, int health)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.SpawnEnemy);
        message.AddInt(id);
        message.AddInt(spawnIndex);
        message.AddInt(health);
        _networkManager.GetServer().SendToAll(message, _networkManager.GetClient().Id);
    }

    public void SendEnemyState(int id, Vector3 pos, Quaternion rot, bool isRunning, float attack)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.EnemyState);
        message.AddInt(id);
        message.AddVector3(pos);
        message.AddQuaternion(rot);
        message.AddBool(isRunning);
        message.AddFloat(attack);
        _networkManager.GetServer().SendToAll(message, _networkManager.GetClient().Id);
    }

    public void SendChangeWave(int waveIndex)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.ChangeWave);
        message.AddInt(waveIndex);
        _networkManager.GetServer().SendToAll(message);
    }

    private static void SendOnClientDead(ushort id)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.PlayerDead);
        message.AddUShort(id);
        _networkManager.GetServer().SendToAll(message);
    }

    public void SendOnClientRespawn(ushort id)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.PlayerRespawn);
        message.AddUShort(id);
        _networkManager.GetServer().SendToAll(message);
    }

    public void SendGameOver(int waveIndex)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.GameOver);
        message.AddInt(waveIndex);
        _networkManager.GetServer().SendToAll(message);
    }
    #endregion

    #region Received
    [MessageHandler((ushort)ClientMessages.MessagesId.Ping)]
    private static void OnClientPing(ushort id, Message message)
    {
        SendPlayerPing(id, message.GetFloat());
    }
    
    [MessageHandler((ushort) ClientMessages.MessagesId.ClientConnected)]
    private static void OnClientConnected(ushort id, Message message)
    {
        SendPlayerConnectedToLobby(id, message.GetULong());
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.StartGame)]
    private static void OnClientStartGame(ushort id, Message message)
    {
        if(id != 1) return;
        SendHostStartGame();
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.Ready)]
    private static void OnClientReady(ushort id, Message message)
    {
        SendInitializeClient(id);
    }
    
    [MessageHandler((ushort) ClientMessages.MessagesId.Inputs)]
    private static void OnClientInputs(ushort id, Message message)
    {
        SendClientInputs(id,message.GetVector3(), message.GetQuaternion(), message.GetFloat(), message.GetFloat(), message.GetVector3(), message.GetBool());
    }

    [MessageHandler((ushort)ClientMessages.MessagesId.Shoot)]
    private static void OnClientShoot(ushort id, Message message)
    {
        SendClientShoot(id, message.GetVector3(), message.GetVector3());   
    }
    
    [MessageHandler((ushort)ClientMessages.MessagesId.ShootEnemy)]
    private static void OnClientShootEnemy(ushort id, Message message)
    {
        SendClientShootEnemy(id,message.GetInt(), message.GetVector3(), message.GetVector3());   
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.Dead)]
    private static void OnClientDead(ushort id, Message message)
    {
        SendOnClientDead(id);
    }
    #endregion
}