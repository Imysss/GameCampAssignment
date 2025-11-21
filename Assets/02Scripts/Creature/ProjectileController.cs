using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private IAttackable _target;
    private float _damage;
    private float _speed;
    private bool _active;

    public void Fire(IAttackable target, float damage, float speed)
    {
        _target = target;
        _damage = damage;
        _speed = speed;
        _active = true;
    }

    private void Update()
    {
        if (!_active || _target == null || _target.IsDead)
        {
            Despawn();
            return;
        }
        
        Vector3 dir = (_target.GetTransform().position - transform.position).normalized;
        transform.position += dir * _speed * Time.deltaTime;
        
        float dist = Vector3.Distance(transform.position, _target.GetTransform().position);

        if (dist < 0.1f)
        {
            _target.TakeDamage(_damage);
            Despawn();
        }
    }

    private void Despawn()
    {
        _active = false;
        PoolManager.Instance.Push(gameObject);
    }
}
