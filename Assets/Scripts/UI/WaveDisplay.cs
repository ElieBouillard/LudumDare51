using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveDisplay : Singleton<WaveDisplay>
{
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private Image _waveCdImage;
    
    private float _waveCd = 10f;

    private void Update()
    {
        if (_waveCd > 0)
        {
            _waveCd -= Time.deltaTime;
            _waveCdImage.fillAmount = _waveCd / 10f;
        }
    }

    public void ChangeWave(int index)
    {
        _waveText.text = $"Wave : {index}";
        _waveCd = 10f;
    }
}
