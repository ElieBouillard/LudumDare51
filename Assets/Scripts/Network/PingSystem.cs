using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class PingSystem : Singleton<PingSystem>
{
    [SerializeField] private TMP_Text _pingText;

    private NetworkManager _networkManager;
    
    private float _time;
    
    public Queue<int> _pingBuffer = new Queue<int>();
    protected override void Awake()
    {
        base.Awake();
        _networkManager = NetworkManager.Instance;
    }
    
    private void Update()
    {
        _time += Time.deltaTime;

        _networkManager.GetClientMessages().SendPing(_time);
    }

    public void ReceivePing(float time)
    {
        _pingBuffer.Enqueue((int)Math.Floor((_time - time)*100));

        if (_pingBuffer.Count > 100)
        {
            _pingBuffer.Dequeue();
        }

        int pingSmooth = 0;
        foreach (var ping in _pingBuffer)
        {
            pingSmooth += ping;
        }

        pingSmooth /= _pingBuffer.Count;

        _pingText.text = $"{pingSmooth} ms";
    }
}