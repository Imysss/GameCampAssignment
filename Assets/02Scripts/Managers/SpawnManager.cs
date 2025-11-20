using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Ally")] 
    [SerializeField] private Tilemap allySpawnTilemap;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private string allyPrefabKey = "ally_default";
    [SerializeField] private float allyYOffset = 1f;

    [Header("Enemy")] 
    [SerializeField] private PathGenerator pathGenerator;
    [SerializeField] private string enemyPrefabKey = "enemy_default";
    [SerializeField] private float enemySpawnInterval = 1f;

    private Coroutine enemySpawnCoroutine;
    
    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {

    }
    
    //AllySpawn
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySpawnAllyAtClick();
        }
    }

    private void TrySpawnAllyAtClick()
    {
        //마우스 위치(스크린) -> 월드 좌표
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        
        //월드 좌표 -> 타일 셀 좌표
        Vector3Int cellPos = allySpawnTilemap.WorldToCell(mouseWorldPos);
        
        //해당 셀에 실제 타일이 있는지 체크
        TileBase clickedTile = allySpawnTilemap.GetTile(cellPos);
        if (clickedTile == null)
        {
            Debug.Log("빈칸 클릭");
            return;
        }
        
        //동료를 소환할 좌표 구하기
        //=> 셀 중앙 월드 좌표 + y축으로 1 정도 올리기
        Vector3 spawnPos = allySpawnTilemap.GetCellCenterWorld(cellPos) + new Vector3(0, 1f, 0);
        
        //동료 소환
        SpawnAlly(allyPrefabKey, spawnPos, Quaternion.identity);
    }

    public GameObject SpawnAlly(string allyKey, Vector3 spawnPos, Quaternion rotation)
    {
        var prefab = ResourceManager.Instance.Load<GameObject>(allyKey);

        if (prefab == null)
        {
            Debug.Log($"[SpawnManager] Ally Prefab not Found: {allyKey}");
            return null;
        }

        GameObject ally = PoolManager.Instance.Pop(prefab, allyKey);
        ally.transform.position = spawnPos;
        ally.transform.rotation = rotation;

        return ally;
    }

    public void StartSpawnEnemyLoop(string key = null)
    {
        //Enemy 자동 스폰 코루틴
        enemySpawnCoroutine = StartCoroutine(CoSpawnEnemyLoop(key));
    }

    public void StopSpawnEnemyLoop()
    {
        if (enemySpawnCoroutine != null)
        {
            StopCoroutine(enemySpawnCoroutine);
        }
    }
    
    //EnemySpawn
    private IEnumerator CoSpawnEnemyLoop(string key = null)
    {
        if (key == null)
            key = enemyPrefabKey;
        
        while (true)
        {
            SpawnEnemy(key);
            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }

    public GameObject SpawnEnemy(string enemyKey)
    {
        var prefab = ResourceManager.Instance.Load<GameObject>(enemyKey);

        if (prefab == null)
        {
            Debug.Log($"[SpawnManager] Enemy Prefab not Found: {enemyKey}");
            return null;
        }

        GameObject enemy = PoolManager.Instance.Pop(prefab, enemyKey);
        
        //Path 적용
        var controller = enemy.GetComponent<IEnemyMovable>();
        if (controller != null)
            controller.InitPath(pathGenerator.worldPath);

        return enemy;
    }
}
