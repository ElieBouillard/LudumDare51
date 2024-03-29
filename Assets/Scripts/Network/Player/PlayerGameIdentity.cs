using CMF;
using UnityEngine;

public class PlayerGameIdentity : PlayerIdentity
{
    public PlayerInputReceiver PlayerInputReceiver;
    public AimingTargetController PlayerAimController;
    public PlayerDistantFiringController PlayerDistantFiringController;

    [SerializeField] private int _initialHealth;

    [SerializeField] private LineRenderer _line;
    private PlayerLocalFiringController _playerFire;
    private AdvancedWalkerController _movementController;
    private Rigidbody _rb;

    [SerializeField] private AudioSource _dieSound;
    
    private int _currHealth;

    private bool _isDead;

    private void Awake()
    {
        _playerFire = GetComponent<PlayerLocalFiringController>();
        _movementController = GetComponent<AdvancedWalkerController>();
        _rb = GetComponent<Rigidbody>();
    }

    public override void Initialize(ushort id, string newName)
    {
        base.Initialize(id, newName);

        if (id == NetworkManager.Instance.GetClient().Id)
        {
            CameraController.Instance.SetTarget(transform);
        }

        _currHealth = _initialHealth;
    }

    public void TakeDamage(int damage)
    {
        _currHealth -= damage;

        SplashScreen.Instance.Splash();
        
        if(IsLocalPlayer())
            HealthBarManager.Instance.SetHealthBarValue((float)_currHealth /(float)_initialHealth);
        
        if (_currHealth <= 0)
        {
           LocalEnable(false);
        }
    }

    public void LocalEnable(bool isRevive)
    {
        if (isRevive)
        {
            CameraController.Instance.SetTarget(transform);
            transform.position = GameManager.Instance.GetSpawnPoint().position;

            _currHealth = _initialHealth;
            HealthBarManager.Instance.SetHealthBarValue((float)_currHealth /(float)_initialHealth);
        }
        else
        {
            CameraController.Instance.SetTarget(null);
            transform.position = new Vector3(0, 1000, 0);
            
            if(!_isDead)
                _dieSound.Play();
                NetworkManager.Instance.GetClientMessages().SendOnDead();
        }
        
        _rb.velocity = Vector3.zero;
        _movementController.enabled = isRevive;
        _playerFire.enabled = isRevive;
        _line.enabled = isRevive;
        _isDead = !isRevive;
    }

    public void DistantEnable(bool isRevive)
    {
        _line.enabled = isRevive;
        _isDead = !isRevive;
        if(!isRevive)
            _dieSound.Play();
    }

    public void LookForSpectate()
    {
        if(!_isDead) return;
        
        foreach (var player in NetworkManager.Instance.GetPlayers())
        {
            if(player.Key == GetId()) continue;
            if (!GameManager.Instance.GetPlayersDead().ContainsKey(player.Key))
            {
                CameraController.Instance.SetTarget(player.Value.transform);
            }
        }
    }
}   
