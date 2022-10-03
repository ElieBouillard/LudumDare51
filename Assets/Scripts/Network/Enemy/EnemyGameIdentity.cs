using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class EnemyGameIdentity : MonoBehaviour
{
    [SerializeField] private int _initialHeatlh = 100;
    [SerializeField] private Image _healthBarImage;

    private EnemyHitbox _hitbox;
    
    private NavMeshAgent _agent;
    private Animator _animator;
    
    private int _id;

    private Transform _target;
    
    private float _currHealth;

    private List<Transform> _players = new List<Transform>();

    private NetworkManager _networkManager;

    public int GetId() => _id;

    public void SetId(int id) => _id = id;
    
    private float _attackTime;

    private Vector3? _targetPos;
    private Quaternion? _targetRot;

    private float _attackCouldown;

    [SerializeField] private AudioSource _die;
    [SerializeField] private AudioSource _idle;
    
    private void Awake()
    {
        _networkManager = NetworkManager.Instance;
        
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        _currHealth = _initialHeatlh;

        _hitbox = GetComponentInChildren<EnemyHitbox>();
        
        if (!_networkManager.GetServer().IsRunning)
        {
            _agent.enabled = false;
            return;
        }

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
        if (_networkManager.GetServer().IsRunning)
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
                    _targetRot = null;
                }
            }
            
            if (_targetRot != null)
            {
                transform.rotation = Quaternion.LerpUnclamped(transform.rotation, _targetRot.Value, Time.deltaTime * 10f);
            }
        }
        else
        {
            if (_targetPos != null)
            {
                transform.position = Vector3.LerpUnclamped(transform.position, _targetPos.Value, Time.deltaTime * 10f);
            }

            if (_targetRot != null)
            {
                transform.rotation = Quaternion.LerpUnclamped(transform.rotation, _targetRot.Value, Time.deltaTime * 10f);
            }
        }

        if (_attackCouldown > 0)
        {
            _attackCouldown -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (_networkManager.GetServer().IsRunning)
        {
            _networkManager.GetServerMessages().SendEnemyState(_id, transform.position, transform.rotation, _animator.GetBool("Run"), _attackTime);
        }
    }

    private void Attack()
    {
        if (_attackTime > 0) return;
        
        _animator.SetTrigger("Attack");

        _targetRot = Quaternion.LookRotation((_target.position - transform.position).normalized, Vector3.up);

        _attackTime = 2f;
    }
    
    private void CheckNearestTarget()
    {
        float distance = Mathf.Infinity;
        Transform bestTarget = null;
        
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i] == null) continue;
            
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
        _die.Play();
        EnemySpawnManager.Instance.GetEnemies().Remove(this);

        GetComponent<Collider>().enabled = false;
        transform.GetChild(0).transform.gameObject.SetActive(false);
        
        Destroy(gameObject, 2f);
    }
    public void ReceivedState(Vector3 pos, Quaternion rot, bool isRunning, float attack)
    {
        _targetPos = pos;
        _targetRot = rot;

        if (_animator.GetBool("Run") != isRunning)
        {
            _animator.SetBool("Run", isRunning);
        }

        if (attack > 0)
        {
            _animator.SetTrigger("Attack") ;
        }
    }

    public void CastAttack()
    {
        List<PlayerGameIdentity> playersHit = _hitbox.GetPlayersHit();

        for (int i = 0; i < playersHit.Count; i++)
        {
            if (playersHit[i].IsLocalPlayer())
            {
                playersHit[i].TakeDamage(50);
            }
        }
    }
}
