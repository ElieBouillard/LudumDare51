using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    [Header("PlayerColor")]
    [Range(0.0f, 10.0f)]
    public float Intensity;
    public Color[] ColorPlayer;
    public Material[] Hologram;


    // Start is called before the first frame update
    void Start()
    {
        changeColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public  void changeColor()
    {
        for (int i = 0; i < ColorPlayer.Length; i++)
        {
            Hologram[i].SetFloat("_Intensity", Intensity);
            Hologram[i].SetColor("_Color",ColorPlayer[i]);
        }


    }
}
