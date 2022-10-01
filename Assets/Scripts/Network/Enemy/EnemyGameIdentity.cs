using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGameIdentity : MonoBehaviour
{
    [SerializeField] private int _initialHeatlh = 100;
    [SerializeField] private Image _healthBarImage;

    private int _id;
    
    private float _currHealth;

    public int GetId() => _id;

    public void SetId(int id) => _id = id;
    
    private void Awake()
    {
        _currHealth = _initialHeatlh;
    }

    public void TakeDamage(int damage)
    {
        _currHealth -= damage;
        
        if (_currHealth <= 0)
        {
            Death();
        }

        _healthBarImage.DOFillAmount(_currHealth / _initialHeatlh, 0.2f);
    }

    private void Death()
    {
        EnemySpawnManager.Instance.GetEnemies().Remove(this);
        Destroy(gameObject);
    }
}
