using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, ISpawnable, IEnemyMovable, IAttackable
{
    //=== 이동 ===
    private List<Vector3> _path;
    private int _pathIdx = 0;

    private bool isMoving = false;

    //=== 스탯 ===
    public float speed = 2f;
    
    public float maxHp = 10f;
    [SerializeField] private float hp;

    public float attackDamage = 5f;
    public float attackInterval = 1f;
    private bool _isAttacking = false;
    private CommanderController _commander;
    
    //=== 스폰 순서 ===
    public int SpawnOrder { get; private set; }

    public void SetSpawnOrder(int order)
    {
        SpawnOrder = order;
    }
    
    public void InitPath(List<Vector3> path)
    {
        _path = path;
        _pathIdx = 0;
        
        // path가 설정되자마자 위치를 초기화
        if (_path != null && _path.Count > 0)
            transform.position = _path[0];
    }

    public void OnSpawn()
    {
        hp = maxHp;
        _pathIdx = 0;
        
        isMoving = true;
        _isAttacking = false;

        _commander = GameManager.Instance.Commander;
    }

    public void OnDespawn()
    {
        StopAllCoroutines();

        isMoving = false;
        _isAttacking = false;
    }

    //==== 이동 처리 ====
    void Update()
    {
        if (!isMoving || _path == null || _path.Count == 0 || _isAttacking)
            return;

        Vector3 target = _path[_pathIdx];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            _pathIdx++;
            
            //엔드 도달
            if (_pathIdx >= _path.Count)
            {
                StartCommanderAttack();
            }
        }
    }
    
    //==== Commander 공격 루프 ====
    private void StartCommanderAttack()
    {
        isMoving = false;
        _isAttacking = true;

        StartCoroutine(CoAttackCommander());
    }

    private IEnumerator CoAttackCommander()
    {
        while (!_commander.IsDead)
        {
            _commander.TakeDamage(attackDamage);
            yield return new WaitForSeconds(attackInterval);
        }
    }
    
    //==== IAttackable 구현 (Ally Projectile이 공격하는 부분) ====
    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
            Die();
    }

    public bool IsDead => hp <= 0;

    public Transform GetTransform() => transform;

    private void Die()
    {
        ResourceManager.Instance.Destroy(gameObject);
    }
}
