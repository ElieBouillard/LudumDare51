using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : Singleton<HealthBarManager>
{
    private Image _healthBarImage;

    protected override void Awake()
    {
        base.Awake();

        _healthBarImage = GetComponent<Image>();
    }

    public void SetHealthBarValue(float value)
    {
        _healthBarImage.DOFillAmount(value, 0.75f);
    } 
}
