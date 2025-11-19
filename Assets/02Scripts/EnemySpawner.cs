using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public PathGenerator PathGenerator;
    public GameObject enemyPrefab;
    public float spawnInterval = 1.0f;

    private void Start()
    {
        StartCoroutine(CoSpawnEnemy());
    }

    private IEnumerator CoSpawnEnemy()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab);
        var ec = enemy.GetComponent<EnemyController>();
        ec.Init(PathGenerator.worldPath);
    }
}
