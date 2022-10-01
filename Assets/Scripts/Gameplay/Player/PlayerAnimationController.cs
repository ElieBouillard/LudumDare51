using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private bool _isLocal;
    
    
    private Animator _animator;
    private CharacterInput _inputs;

    private float _velocityZ;
    private float _velocityX;

    public float GetVelocityZ() => _velocityZ;
    public float GetVelocityX() => _velocityX;

    public void SetVelocityZ(float velocityZ) => _velocityZ = velocityZ; 
    public void SetVelocityX(float velocityX) => _velocityX = velocityX; 
    
    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _inputs = gameObject.GetComponentInParent<CharacterInput>();
    }

    private void Update()
    {
        if (_isLocal)
        {
            Vector3 input = new Vector3(_inputs.GetHorizontalMovementInput(), 0f , _inputs.GetVerticalMovementInput());
        
            _velocityZ = Vector3.Dot(input.normalized, transform.forward);
            _velocityX = Vector3.Dot(input.normalized, transform.right);
        }
        
        _animator.SetFloat("VelocityZ", _velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", _velocityX, 0.1f, Time.deltaTime);
    }
}
