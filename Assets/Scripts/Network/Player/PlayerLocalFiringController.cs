using System;
using CameraShake;
using Steamworks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerLocalFiringController : MonoBehaviour
{
    [SerializeField] private Transform _lineTransform;
    [SerializeField] private VisualEffect _muzzleFlash;
    [SerializeField] private GameObject _impactPrefab;
    
    [SerializeField] private int _damage;
    
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_lineTransform.position, _lineTransform.forward, out RaycastHit hit, Mathf.Infinity))
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

    private void Shoot(Vector3 pos, Vector3 dir)
    {
        ShootFx(pos, dir);
        
        NetworkManager.Instance.GetClientMessages().SendShoot(pos,dir);
    }

    private void Shoot(EnemyGameIdentity enemy, Vector3 pos, Vector3 dir)
    {
        ShootFx(pos, dir);

        enemy.TakeDamage(_damage);
        
        NetworkManager.Instance.GetClientMessages().SendShootEnemy(enemy.GetId(), pos,dir);
    }

    private void ShootFx(Vector3 pos, Vector3 dir)
    {       
        CameraShaker.Presets.ShortShake3D();
        
        // _muzzleFlash.Play();
        
        GameObject impactTemp = Instantiate(_impactPrefab, pos, Quaternion.identity);
        impactTemp.transform.forward = dir;
        Destroy(impactTemp, 2f);
    }
}