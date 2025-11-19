using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2f;
    private List<Vector3> _path;
    private int _currentIdx = 0;

    public void Init(List<Vector3> path)
    {
        _path = path;
        _currentIdx = 0;
        transform.position = _path[0];
    }

    void Update()
    {
        if (_path == null || _path.Count == 0)
            return;

        Vector3 target = _path[_currentIdx];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            _currentIdx++;
            
            //엔드 도달
            if (_currentIdx >= _path.Count)
            {
                Destroy(gameObject);
            }
        }
    }
}
