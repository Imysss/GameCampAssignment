using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyController : MonoBehaviour, ISpawnable
{
    public AllyStat stat;

    private float _cooldown = 0f;
    private IAttackable _currentTarget;

    [SerializeField] private MonoBehaviour targetDebug;

    private Vector3Int currentCell;
    
    private static readonly Collider2D[] _results = new Collider2D[200];

    public void OnSpawn()
    {
        _cooldown = 0f;
        _currentTarget = null;
    }

    public void OnDespawn()
    {
        _currentTarget = null;
        SpawnManager.Instance.ReleaseCell(currentCell);
    }

    private void Update()
    {
        if (stat == null)
            return;

        _cooldown -= Time.deltaTime;
        
        //타겟 없거나, 죽었거나, 범위를 벗어나면 자동 재선택
        if (_currentTarget == null || _currentTarget.IsDead || !IsInRange(_currentTarget))
        {
            _currentTarget = FindTarget();
        }

        if (_currentTarget == null)
        {
            Debug.Log("No target found");
            return;
        }
        
        //공격 가능하면 공격
        if (_cooldown <= 0f)
        {
            Attack(_currentTarget);
        }
        
        targetDebug = _currentTarget as MonoBehaviour;
    }

    public void RegisterCell(Vector3Int cell)
    {
        currentCell = cell;
    }

    private bool IsInRange(IAttackable target)
    {
        return Vector3.Distance(transform.position, target.GetTransform().position) <= stat.attackRange;
    }

    private IAttackable FindTarget()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, stat.attackRange, _results);

        IAttackable selected = null;
        int bestOrder = int.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var enemy = _results[i].GetComponent<IAttackable>();
            if (enemy == null || enemy.IsDead)
                continue;

            if (enemy.SpawnOrder < bestOrder)
            {
                bestOrder = enemy.SpawnOrder;
                selected = enemy;
            }
        }
        return selected;
    }

    private void Attack(IAttackable target)
    {
        _cooldown = 1f / stat.attackSpeed;
        target.TakeDamage(stat.attack);
    }

    private void OnDrawGizmosSelected()
    {
        if (stat == null)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stat.attackRange);
    }
}
