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
    [SerializeField] private LayerMask allyLayer;
    
    //현재 배치된 모든 ally의 cell 좌표 지정
    private HashSet<Vector3Int> occupiedAllyCells = new();

    [Header("Enemy")] 
    [SerializeField] private PathGenerator pathGenerator;
    [SerializeField] private string enemyPrefabKey = "enemy_default";
    [SerializeField] private float enemySpawnInterval = 1f;

    private int enemySpawnCounter = 0;

    private Coroutine enemySpawnCoroutine;
    
    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    
    //AllySpawn
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySpawnAllyAtClick();
        }
    }

    //==== Ally ====
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
        
        //이미 해당 셀에 Ally 존재 -> 생성 금지
        if (occupiedAllyCells.Contains(cellPos))
        {
            Debug.Log("해당 타일에 이미 Ally가 존재합니다.");
            return;
        }
        
        //동료를 소환할 좌표 구하기
        //=> 셀 중앙 월드 좌표 + y축으로 1 정도 올리기
        Vector3 spawnPos = allySpawnTilemap.GetCellCenterWorld(cellPos) + new Vector3(0, allyYOffset, 0);
        
        
        //동료 소환
        GameObject ally = SpawnAlly(allyPrefabKey, spawnPos, Quaternion.identity);
        
        //동료 소환 성공 시 해당 셀을 점유 상태로 등록
        if (ally != null)
        {
            occupiedAllyCells.Add(cellPos);
            
            //Ally가 죽을 때 Cell을 반납할 수 있도록 컴포넌트도 함께 넘김
            AllyController ac = ally.GetComponent<AllyController>();
            if (ac != null)
            {
                ac.RegisterCell(cellPos);
            }
        }
    }

    public GameObject SpawnAlly(string allyKey, Vector3 spawnPos, Quaternion rotation)
    {

        GameObject ally = ResourceManager.Instance.Instantiate(allyKey, pooling: true);
        ally.transform.position = spawnPos;
        ally.transform.rotation = rotation;

        return ally;
    }

    public void ReleaseCell(Vector3Int cell)
    {
        if (occupiedAllyCells.Contains(cell))
        {
            occupiedAllyCells.Remove(cell);
        }
    }

    
    //==== Enemy ====
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
        GameObject enemy = ResourceManager.Instance.Instantiate(enemyKey, pooling: true);
        
        //Path 적용
        var movable = enemy.GetComponent<IEnemyMovable>();
        if (movable != null)
        {
            movable.InitPath(pathGenerator.worldPath);
        }
        
        //SpawnOrder 적용
        var controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.SetSpawnOrder(enemySpawnCounter++);
        }

        return enemy;
    }
}
