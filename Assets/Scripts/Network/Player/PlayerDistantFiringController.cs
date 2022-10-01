using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerDistantFiringController : MonoBehaviour
{
    [SerializeField] private VisualEffect _muzzleFlash;
    [SerializeField] private GameObject _impactPrefab;

    public void ShootEnemy(EnemyGameIdentity enemy, Vector3 pos, Vector3 dir)
    {
        ShootFx(pos, dir);
    }

    public void ShootFx(Vector3 pos,Vector3 dir)
    {
        _muzzleFlash.Play();
        GameObject impactTemp = Instantiate(_impactPrefab, pos, Quaternion.identity);
        impactTemp.transform.forward = dir;
        Destroy(impactTemp, 2f);
    }
}