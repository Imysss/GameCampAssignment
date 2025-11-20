using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMovable
{
    void InitPath(List<Vector3> worldPath);
}
