using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Steamworks;
using TMPro;
using UnityEngine;

public class PanelManager : Singleton<PanelManager>
{
    [SerializeField] private Panel[] _panels;

    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _camStartMenuPos;
    [SerializeField] private Transform _camLobbyPos;
    
    
    private NetworkManager _networkManager;
    private bool _isPause;
    protected override void Awake()
    {
        base.Awake();

        EnablePanel(PanelType.MainMenu);
    }

    private void Start()
    {
        _networkManager = NetworkManager.Instance;
    }

    private void Update()
    {
        if (_networkManager.GetGameState() == GameState.Gameplay)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                foreach (var panel in _panels)
                {
                    if (panel.PanelType == PanelType.Pause) panel.gameObject.SetActive(!_isPause);
                    _isPause = !_isPause;
                }
            }
        }
    }

    public void DisablePauseMenu()
    {
        _isPause = false;
    }
    
    public void EnablePanel(PanelType panelType)
    {
        foreach (var panel in _panels)
        {
            panel.gameObject.SetActive(panel.PanelType == panelType);
        }

        if (panelType == PanelType.MainMenu)
        {
            _camera.transform.DOMove(_camStartMenuPos.position, 2f);
            _camera.transform.DORotate(_camStartMenuPos.rotation.eulerAngles, 2f);
        }
        
        if (panelType == PanelType.Lobby)
        {
            _camera.transform.DOMove(_camLobbyPos.position, 2f);
            _camera.transform.DORotate(_camLobbyPos.rotation.eulerAngles, 2f);
        }
    }
}

public enum PanelType
{
    MainMenu = 1,
    Options,
    Lobby,
    Pause,
}