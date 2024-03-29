using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _controlsDropdown;

    private void Awake()
    {
        if(PlayerPrefs.HasKey("Controls"))
            _controlsDropdown.value = PlayerPrefs.GetInt("Controls");
    }

    public void OnDropDownChange(int value)
    {
        PlayerPrefs.SetInt("Controls", value);

        if (NetworkManager.Instance == null) return;

        NetworkManager.Instance.GetLocalPlayer().GetComponent<CharacterKeyboardInput>().ControlId = value;
    }
}
