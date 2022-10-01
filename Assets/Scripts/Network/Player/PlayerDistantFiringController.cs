using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerDistantFiringController : MonoBehaviour
{
    [SerializeField] private VisualEffect _muzzleFlash;
    [SerializeField] private GameObject _impactPrefab;
    [SerializeField] private int _damage;
    
    
    public void ShootEnemy(int id, Vector3 pos, Vector3 dir)
    {
        foreach (var enemy in EnemySpawnManager.Instance.GetEnemies())
        {
            if (enemy.GetId() == id)
            {
                enemy.TakeDamage(_damage);
                return;
            }
        }
        
        ShootFx(pos, dir);
    }

    public void ShootFx(Vector3 pos,Vector3 dir)
    {
        // _muzzleFlash.Play();
        GameObject impactTemp = Instantiate(_impactPrefab, pos, Quaternion.identity);
        impactTemp.transform.forward = dir;
        Destroy(impactTemp, 2f);
    }
}