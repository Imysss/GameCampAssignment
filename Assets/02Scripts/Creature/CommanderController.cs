using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderController : MonoBehaviour, IAttackable
{
    public float maxHp = 100f;
    [SerializeField] private float _currentHp;
    
    public bool IsDead => _currentHp <= 0;
    public Transform GetTransform() => transform;
    public int SpawnOrder { get; }
    
    private void Awake()
    {
        _currentHp = maxHp;
    }
    

    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;

        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.OnCommanderDead();
        Destroy(gameObject);
    }
}
