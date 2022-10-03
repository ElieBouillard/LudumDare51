using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreen : Singleton<SplashScreen>
{
    private Image _image;
    
    private void Start()
    {
        _image = GetComponent<Image>();
    }

    public void Splash()
    {
        _image.DOFade(1, 0.2f);
        _image.DOFade(0, 1f).SetDelay(2f);
    }
}
