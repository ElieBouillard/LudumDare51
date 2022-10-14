using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class IpAddressInput : Singleton<IpAddressInput>
{ 
    private TMP_InputField _ipAddressInputField;

    protected override void Awake()
    {
        base.Awake();

        _ipAddressInputField = GetComponent<TMP_InputField>();
    }

    public string GetIpAddress()
    {
        return _ipAddressInputField.text;
    }
}
