using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPlayer_ : MonoBehaviour
{

    private Animator animator;

    [SerializeField] private GameObject Ennemyexploded;
    [SerializeField] private GameObject[] Decals;
    [Header("------------------- Fx -----------------")]
    [SerializeField] private PSSpawnData[] _Fx;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<Animator>())
            animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("explode");
            Explode();
        }
          
    }


    void Delete()
    {
        Destroy(gameObject);


    }

    void BloodSpawn()
    {
        _Fx.PlayFX("Blood");
    }
    void Explode()
    {
        _Fx.PlayFX("Explode");
    }

    void instanciatemeshExploded()
    {
        if(Ennemyexploded)
            Instantiate(Ennemyexploded, transform.position, transform.rotation);
    }
    void DecalsSpawn()
    {
        if(Decals != null)
        {
            int rand = Random.Range(0, Decals.Length);
            Decals[rand].SetActive(true);
        }

    }
}
