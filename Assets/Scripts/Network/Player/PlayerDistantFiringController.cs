using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerDistantFiringController : MonoBehaviour
{
    [SerializeField] private VisualEffect _muzzleFlash;
    [SerializeField] private GameObject _impactPrefab;
    [SerializeField] private GameObject _impactBloodPrefab;
    [SerializeField] private int _damage;
    
    [SerializeField] private AudioSource[] _shotSources;
    [SerializeField] private AudioClip[] _shotClips;
    
    public void ShootEnemy(int id, Vector3 pos, Vector3 dir)
    {
        PlayShotSound();
        
        GameObject impactTemp = Instantiate(_impactBloodPrefab, pos, Quaternion.identity);
        impactTemp.transform.forward = dir;
        Destroy(impactTemp, 2f);
        
        GameManager.Instance.AddScore(GetComponent<PlayerIdentity>().GetId());
        
        _muzzleFlash.Play();
        
        // ShootFx(pos, dir);
        
        foreach (var enemy in EnemySpawnManager.Instance.GetEnemies())
        {
            if (enemy.GetId() == id)
            {
                enemy.TakeDamage(_damage);
                return;
            }
        }
    }

    private int _shotSoundIndex = 0;
    
    public void ShootFx(Vector3 pos,Vector3 dir)
    {
        PlayShotSound();
        
        _muzzleFlash.Play();
        GameObject impactTemp = Instantiate(_impactPrefab, pos, Quaternion.identity);
        impactTemp.transform.forward = dir;
        Destroy(impactTemp, 2f);
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
}