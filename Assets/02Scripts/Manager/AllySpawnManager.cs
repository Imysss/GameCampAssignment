using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AllySpawnManager : MonoBehaviour
{
    private Camera mainCamera;
    public Tilemap tilemap;        //바닥 타일맵
    public GameObject allyPrefab;   //소환할 동료 프리팹

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnAllyOnClickTile();
        }
    }

    void SpawnAllyOnClickTile()
    {
        //마우스 위치(스크린) -> 월드 좌표
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        
        //월드 좌표 -> 타일 셀 좌표
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);
        
        //해당 셀에 실제 타일이 있는지 체크
        TileBase clickedTile = tilemap.GetTile(cellPos);
        if (clickedTile == null)
        {
            Debug.Log("빈칸 클릭");
            return;
        }
        
        //동료를 소환할 좌표 구하기
        //=> 셀 중앙 월드 좌표 + y축으로 1 정도 올리기
        Vector3 spawnPos = tilemap.GetCellCenterWorld(cellPos) + new Vector3(0, 1f, 0);
        
        //동료 소환
        Instantiate(allyPrefab, spawnPos, Quaternion.identity);
    }
}
