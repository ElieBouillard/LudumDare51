using System;
using CameraShake;
using Steamworks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class PlayerLocalFiringController : MonoBehaviour
{
    [SerializeField] private Transform _lineTransform;
    [SerializeField] private VisualEffect _muzzleFlash;
    [SerializeField] private GameObject _impactPrefab;
    [SerializeField] private GameObject _impactBloodPrefab;
    
    [SerializeField] private int _damage;

    [SerializeField]private LayerMask _layerMask;

    [SerializeField] private AudioSource[] _shotSources;
    [SerializeField] private AudioClip[] _shotClips;
    [SerializeField] private AudioSource _hitmarker;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_lineTransform.position, _lineTransform.forward, out RaycastHit hit, Mathf.Infinity, _layerMask))
            {
                if (hit.collider.TryGetComponent(out EnemyGameIdentity enemy))
                {
                    Shoot(enemy, hit.point, hit.normal);
                }
                else
                { 
                    Shoot(hit.point, hit.normal);
                }
            }
        }
    }

    private int _shotSoundIndex = 0;
    
    private void Shoot(Vector3 pos, Vector3 dir)
    {
        ShootFx(pos, dir);

        PlayShotSound();
        
        NetworkManager.Instance.GetClientMessages().SendShoot(pos,dir);
    }

    private void Shoot(EnemyGameIdentity enemy, Vector3 pos, Vector3 dir)
    {
        // ShootFx(pos, dir);

        _hitmarker.Play();
        
        CameraShaker.Presets.ShortShake3D();
        
        PlayShotSound();
        
        _muzzleFlash.Play();
        
        GameObject impactTemp = Instantiate(_impactBloodPrefab, pos, Quaternion.identity);
        impactTemp.transform.forward = dir;
        Destroy(impactTemp, 2f);
        
        enemy.TakeDamage(_damage);
        
        GameManager.Instance.AddScore(NetworkManager.Instance.GetLocalPlayer().GetId());
        
        NetworkManager.Instance.GetClientMessages().SendShootEnemy(enemy.GetId(), pos,dir);
    }


    private void PlayShotSound()
    {
        _shotSources[_shotSoundIndex].PlayOneShot(_shotClips[Random.Range(0,3)]);
        _shotSources[_shotSoundIndex].pitch = Random.Range(0.9f, 1.1f);
        _shotSoundIndex++;
        
        if (_shotSoundIndex > 2)
        {
            _shotSoundIndex = 0;
        }
    }
    
    private void ShootFx(Vector3 pos, Vector3 dir)
    {       
        CameraShaker.Presets.ShortShake3D();
        
        GameObject impactTemp = Instantiate(_impactPrefab, pos, Quaternion.identity);
        impactTemp.transform.forward = dir;
        Destroy(impactTemp, 2f);
        
        _muzzleFlash.Play();
    }
}