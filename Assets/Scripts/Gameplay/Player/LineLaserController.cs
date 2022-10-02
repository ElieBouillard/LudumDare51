using System;
using UnityEngine;

public class LineLaserController : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward,out RaycastHit hit, Mathf.Infinity, _layerMask))
        {
            _lineRenderer.SetPosition(1, new Vector3(0, 0, hit.distance) );
        }
        else
        {
            _lineRenderer.SetPosition(1,new Vector3(0, 0, 150f));
        }
    }
}
