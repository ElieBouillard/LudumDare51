using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyServerGameIdentity : MonoBehaviour
{
    [SerializeField] private int _initialHeatlh = 100;
    [SerializeField] private Image _healthBarImage;

    private NavMeshAgent _agent;
    private Animator _animator;
    
    private int _id;

    private Transform _target;
    
    private float _currHealth;

    private List<Transform> _players = new List<Transform>();
    
    public int GetId() => _id;

    public void SetId(int id) => _id = id;
    
    private float _attackTime;
    
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        _currHealth = _initialHeatlh;

        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForSeconds(1f);
        
        foreach (var player in NetworkManager.Instance.GetPlayers())
        {
            _players.Add(player.Value.transform);
        }
    }

    private void Update()
    {
        _animator.SetBool("Run", _agent.velocity.magnitude > 0.05f);
        
        CheckNearestTarget();

        if(_target == null) return;
        
        float distance = (_target.position - transform.position).magnitude;
        
        if (distance < 1.5f)
        {
            _agent.isStopped = true;
            Attack();
        }

        if (_attackTime > 0)
        {
            _attackTime -= Time.deltaTime;
        }
        else
        {
            if (_agent.isStopped)
            {
                _agent.isStopped = false;
            }
        }
    }

    private void Attack()
    {
        if (_attackTime > 0) return;
        
        _animator.SetTrigger("Attack");

        _attackTime = 2f;
    }
    
    private void CheckNearestTarget()
    {
        float distance = Mathf.Infinity;
        Transform bestTarget = null;
        
        for (int i = 0; i < _players.Count; i++)
        {
            float currDistance = (_players[i].position - transform.position).magnitude;
            
            if (currDistance < distance)
            {
                bestTarget = _players[i];
                distance = currDistance;
            }
        }

        if(bestTarget == null) return;

        _target = bestTarget;
        
        _agent.SetDestination(_target.position);
    }
    
    public void TakeDamage(int damage)
    {
        _currHealth -= damage;
        
        if (_currHealth <= 0)
        {
            Death();
        }

        // _healthBarImage.DOFillAmount(_currHealth / _initialHeatlh, 0.2f);
    }

    private void Death()
    {
        EnemySpawnManager.Instance.GetEnemies().Remove(this);
        Destroy(gameObject);
    }
}
