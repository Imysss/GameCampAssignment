using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, ISpawnable, IEnemyMovable, IAttackable
{
    //=== 이동 ===
    private List<Vector3> _path;
    private int _pathIdx = 0;

    private bool isMoving = false;
    private bool _isAttacking = false;
    
    public float speed = 2f;
    public float blockDistance = 0.6f;  //목적지 도달 후 enemy 간 간격

    private EnemyController _front;
    
    //=== 체력 ===
    public float maxHp = 10f;
    [SerializeField] private float hp;

    public float attackDamage = 5f;
    public float attackInterval = 1f;

    private CommanderController _commander;
    
    //=== 스폰 순서 ===
    public int SpawnOrder { get; private set; }
    public void SetSpawnOrder(int order) => SpawnOrder = order;
    
    public bool IsDead => hp <= 0;
    public Transform GetTransform() => transform;
    
    //=== Event ===
    public static Action<EnemyController> OnEnemyKilled;
    
    public void InitPath(List<Vector3> path)
    {
        _path = path;
        _pathIdx = 0;
        
        //path가 설정되자마자 위치를 초기화
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
        EnemyManager.Instance.Register(this);
    }

    public void OnDespawn()
    {
        EnemyManager.Instance.Unregister(this);
        
        StopAllCoroutines();

        isMoving = false;
        _isAttacking = false;
    }

    //==== 이동 처리 ====
    void Update()
    {
        if (!isMoving || _path == null || _pathIdx > _path.Count || _isAttacking)
            return;
        
        //앞 Enemy 찾기
        if (CanMove())
        {
            MoveForward();
        }
    }

    private bool CanMove()
    {
        //앞 enemy 찾기
        _front = EnemyManager.Instance.GetFrontEnemy(this);
        
        //내가 더 앞에 있으면 그냥 이동
        if (_front == null)
            return true;
        
        //앞 enemy와의 거리 체크
        float dist = Vector3.Distance(transform.position, _front.transform.position);
        if (dist < blockDistance)
        {
            //너무 가까우면 멈추기
            return false;
        } 
        
        //충분히 멀면 이동
        return true;
    }

    private void MoveForward()
    {
        Vector3 target = _path[_pathIdx];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            _pathIdx++;

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



    private void Die()
    {
        //WaveManager 구독 중
        OnEnemyKilled?.Invoke(this);
        
        ResourceManager.Instance.Destroy(gameObject);
    }
}
