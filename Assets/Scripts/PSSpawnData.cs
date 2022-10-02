using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PSSpawnData
{
    public string Name { get => _name; }

    [SerializeField] private string _name;
    [SerializeField] private GameObject _psPrefab;
    [SerializeField] public Transform _parent;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private bool _stickToParent;

    public void SpawnFX()
    {
        GameObject daObject;
        if (_parent != null)
            daObject = GameObject.Instantiate(_psPrefab, _parent.position + _offset, Quaternion.Euler(_parent.rotation.eulerAngles + _rotation), _parent);
        else
            daObject = GameObject.Instantiate(_psPrefab, _offset, Quaternion.Euler(_rotation));
        if (_stickToParent)
            daObject.transform.SetParent(null);
        daObject.GetComponent<ParticleSystem>().Play();

    }

    public void SpawnFXtransform(Transform transform)
    {
        GameObject daObject;
        if (transform != null)
            daObject = GameObject.Instantiate(_psPrefab, transform.position + _offset, Quaternion.Euler(transform.rotation.eulerAngles + _rotation), transform);
        else
            daObject = GameObject.Instantiate(_psPrefab, _offset, Quaternion.Euler(_rotation));
        if (_stickToParent)
            daObject.transform.SetParent(null);
        daObject.GetComponent<ParticleSystem>().Play();

    }
}

public static class PSSpawner
{
    public static void PlayFX(this PSSpawnData[] ps, string name)
    {
        foreach (PSSpawnData psSpawnData in ps)
        {
            if (psSpawnData.Name == name)
            {
                psSpawnData.SpawnFX();
            }
        }
    }

    public static void PlayFXTransform(this PSSpawnData[] ps, string name, Transform transform)
    {
        foreach (PSSpawnData psSpawnData in ps)
        {
            if (psSpawnData.Name == name)
            {
                psSpawnData.SpawnFXtransform(transform);
            }
        }
    }
}