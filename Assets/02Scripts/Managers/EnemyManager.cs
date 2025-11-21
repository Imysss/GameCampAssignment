using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private readonly List<EnemyController> _enemies = new();

    public void Register(EnemyController enemy)
    {
        _enemies.Add(enemy);
    }

    public void Unregister(EnemyController enemy)
    {
        _enemies.Remove(enemy);
    }
    
    //현재 enemy 앞에 있는 enemy 중 가장 앞 (늘 spawn order가 작은 녀석이 앞에 오게 되어 있음)
    public EnemyController GetFrontEnemy(EnemyController self)
    {
        int idx = _enemies.IndexOf(self);
        if (idx <= 0)
            return null;

        return _enemies[idx - 1];
    }
}
