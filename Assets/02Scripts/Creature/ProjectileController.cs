using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Vector3 _direction;
    private float _damage;
    private float _speed;
    private bool _active;
    
    //투사체 생존 시간
    [SerializeField] private float _maxLifetime = 4f;
    private float _lifetime;

    public void Fire(IAttackable target, float damage, float speed)
    {
        _direction = (target.GetTransform().position - transform.position).normalized;
        _damage = damage;
        _speed = speed;
        _active = true;
        
        _lifetime = _maxLifetime;
    }

    private void Update()
    {
        if (!_active)
            return;
        
        //life time 체크
        _lifetime -= Time.deltaTime;
        if (_lifetime <= 0)
        {
            Despawn();
            return;
        }
        
        transform.position += _direction * (_speed * Time.deltaTime);
    }

    private void Despawn()
    {
        _active = false;
        PoolManager.Instance.Push(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_active)
            return;

        if (other.TryGetComponent<EnemyController>(out var enemy))
        {
            enemy.TakeDamage(_damage);
            Despawn();
        }
    }
}
