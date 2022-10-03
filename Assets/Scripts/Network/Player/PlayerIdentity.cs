using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerIdentity : MonoBehaviour
{
    #region Fields
    private ushort _id;
    protected ulong _steamId;
    private bool _isLocalPlayer;
    #endregion
    
    #region Getters
    public ushort GetId() => _id;
    public ulong GetSteamId() => _steamId;
    public bool IsLocalPlayer() => _isLocalPlayer;
    #endregion

    [SerializeField] private GameObject _mesh;
    
    public virtual void Initialize(ushort id, string newName)
    {
        _id = id;

        if (_id == NetworkManager.Instance.GetClient().Id) { _isLocalPlayer = true; }

        gameObject.name = newName;
        
        for (int i = 0; i < NetworkManager.Instance.GetPlayers().ToList().Count; i++)
        {
            if (NetworkManager.Instance.GetPlayers().Keys.ToList()[i] == GetId())
            {
                if (i == 0)
                {
                    _mesh.layer = LayerMask.NameToLayer("Player");
                }
                if (i == 1)
                {
                    _mesh.layer = LayerMask.NameToLayer("Player2");
                }
                if (i == 2)
                {
                    _mesh.layer = LayerMask.NameToLayer("Player3");
                }
                if (i == 3)
                {
                    _mesh.layer = LayerMask.NameToLayer("Player4");
                }
                break;
            }
        }
    }
    
    public virtual void Initialize(ushort id, ulong steamId)
    {
        Initialize(id, SteamFriends.GetFriendPersonaName((CSteamID)steamId));
        
        _steamId = steamId;
    }
}