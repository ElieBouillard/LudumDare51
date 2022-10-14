using System;
using CameraShake;
using Steamworks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class PlayerLocalFiringController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _lineTransform;
    [SerializeField] private VisualEffect _muzzleFlash;
    [SerializeField] private GameObject _impactPrefab;
    [SerializeField] private GameObject _impactBloodPrefab;
    
    [Header("Parameters")]
    [SerializeField] private int _damage;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _reloadTime;
    
    
    [SerializeField]private LayerMask _layerMask;

    [Header("Audio")]
    [SerializeField] private AudioSource[] _shotSources;
    [SerializeField] private AudioClip[] _shotClips;
    [SerializeField] private AudioSource _hitmarker;

    private AmmoCanvas _ammoCanvas;

    private bool _canFire = true;
    private bool _isFiring;
    private float _fireCd;
    private float _reloadCD;
    private int _ammouCount = 30;

    private void Awake()
    {
        _ammoCanvas = AmmoCanvas.Instance;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isFiring = true;
            _fireCd = 0;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isFiring = false;
        }

        if (_isFiring)
        {
            if (_fireCd > 0)
            {
                _fireCd -= Time.deltaTime;
            }
            else
            {
                _fireCd = _fireRate;
                if (_canFire)
                {
                    Fire();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_reloadCD <= 0)
            {
                StartReload();
            }
        }

        if (_reloadCD > 0)
        {
            _reloadCD -= Time.deltaTime;
        }
        else if(_reloadCD != -1)
        {
            _reloadCD = -1;
            OnReloaded();
        }
        
    }

    private void Fire()
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

            _ammouCount--;
            _ammoCanvas.SetAmmoCount(_ammouCount);

            if (_ammouCount <= 0)
            {
                StartReload();
            }
            
            if (_ammouCount == 0)
                _canFire = false;
        }
    }

    private void StartReload()
    {
        _reloadCD = _reloadTime;
        _ammoCanvas.Reload(_reloadTime);
        _canFire = false;
    }

    private void OnReloaded()
    {
        _ammouCount = 30;
        _ammoCanvas.SetAmmoCount(_ammouCount);
        _canFire = true;
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