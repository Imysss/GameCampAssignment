using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/AllyStat")]
public class AllyStat : ScriptableObject
{
    public string id;
    public float attack;
    public float attackRange;
    public float attackSpeed;

    [Header("Projectile Settings")] 
    public string projectileKey;
    public float projectileSpeed;
}
