using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void TakeDamage(float amount);
    bool IsDead { get; }
    Transform GetTransform();
    int SpawnOrder { get; } //스폰 순서 우선 공격
}
