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

    private static void SendClientInputs(ushort id, Vector3 pos, Quaternion rot, float velocityZ, float velocityX, Vector3 aimPos)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.PlayerInputs);
        message.AddUShort(id);
        message.AddVector3(pos);
        message.AddQuaternion(rot);
        message.AddFloat(velocityZ);
        message.AddFloat(velocityX);
        message.AddVector3(aimPos);
        _networkManager.GetServer().SendToAll(message, id);
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
        SendClientInputs(id,message.GetVector3(), message.GetQuaternion(), message.GetFloat(), message.GetFloat(), message.GetVector3());
    }
    #endregion
}