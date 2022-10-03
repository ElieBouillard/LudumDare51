using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    #region Singleton

    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(SoundManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }

    #endregion

    [Header("References")]
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _vfxVolumeSlider;
    [SerializeField] private AudioMixer _audioMixer;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _musicVolumeSlider.onValueChanged.AddListener(OnChangeMusicVolumeValue);
        _vfxVolumeSlider.onValueChanged.AddListener(OnChangeVFXVolumeValue);

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            _audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
            _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }


        if (PlayerPrefs.HasKey("VfxVolume"))
        {            
            _audioMixer.SetFloat("VfxVolume", PlayerPrefs.GetFloat("VfxVolume"));
            _vfxVolumeSlider.value = PlayerPrefs.GetFloat("VfxVolume");
        }

    }

    private void OnChangeMusicVolumeValue(float value)
    {
        _audioMixer.SetFloat("MusicVolume", value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }    
    private void OnChangeVFXVolumeValue(float value)
    {
        _audioMixer.SetFloat("VfxVolume", value);
        PlayerPrefs.SetFloat("VfxVolume", value);
    }
    
}