using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : Singleton<GameOverPanel>
{
    [SerializeField] private GameObject _parent;
    [SerializeField] private PlayerEndGame _playerProfileScore;
    [SerializeField] private Button _leaveButton;

    private Vector3 Offset = new Vector3(0,150,0);

    private void Start()
    {
        _leaveButton.onClick.AddListener(NetworkManager.Instance.Leave);
    }

    public void EnableGameOverPanel()
    {
        _parent.SetActive(true);
    }

    public void AddScore(ushort id, int score)
    {
        PlayerEndGame playerEndGame = Instantiate(_playerProfileScore, _parent.transform);
        playerEndGame.transform.localPosition = Offset;
        Offset.y -= 150;

        if (NetworkManager.Instance.GetUseSteam())
        {
            playerEndGame.Initialize((CSteamID)NetworkManager.Instance.GetPlayers()[id].GetSteamId(), score);
        }
        else
        {
            playerEndGame.Initialize(NetworkManager.Instance.GetPlayers()[id].name, score);
        }
    } 
    
    
}
