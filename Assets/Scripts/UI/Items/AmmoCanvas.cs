using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCanvas : Singleton<AmmoCanvas>
{
    [SerializeField] private TMP_Text _bulletsText;
    [SerializeField] private GameObject _reloadBarObject;
    [SerializeField] private Image _reloadBarImage;

    protected override void Awake()
    {
        base.Awake();
        
        _reloadBarObject.SetActive(false);
    }

    public void SetAmmoCount(int ammoCount)
    {
        _bulletsText.text = $"{ammoCount}/30";
    }

    public void Reload(float reloadTime)
    {
        _reloadBarObject.SetActive(true);
        _reloadBarImage.fillAmount = 0f;
        _reloadBarImage.DOKill();
        _reloadBarImage.DOFillAmount(1f, reloadTime);
        StartCoroutine(EnReload(reloadTime));
    }


    private IEnumerator EnReload(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);
        _reloadBarObject.SetActive(false);
    }
}
