using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, ISpawnable, IEnemyMovable
{
    public float speed = 2f;
    private List<Vector3> _path;
    private int _idx = 0;

    private bool isMoving = false;

    public void InitPath(List<Vector3> path)
    {
        _path = path;
        _idx = 0;
    }

    public void OnSpawn()
    {
        _idx = 0;
        
        //이동 루틴 시작
        isMoving = true;
    }

    public void OnDespawn()
    {
        //상태 초기화
        isMoving = false;
    }


    void Update()
    {
        if (!isMoving)
            return;
        if (_path == null || _path.Count == 0)
            return;

        if (_idx == 0 && transform.position != _path[0])
        {
            transform.position = _path[0];
        }

        Vector3 target = _path[_idx];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            _idx++;
            
            //엔드 도달
            if (_idx >= _path.Count)
            {
                Destroy(gameObject);
            }
        }
    }
}
