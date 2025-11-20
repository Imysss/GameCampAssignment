using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathGenerator : MonoBehaviour
{
    public Tilemap pathTilemap;     //길 타일맵
    public Transform spawnPoint;    //시작 위치
    public Transform endPoint;      //도착 위치

    public List<Vector3> worldPath; //적들이 따라갈 최종 월드 좌표 경로

    private void Awake()
    {
        GeneratePath();
    }

    private void GeneratePath()
    {
        Debug.Log("Generate path");
        
        worldPath = new List<Vector3>();
        
        //스폰, 엔드 셀 좌표 구하기
        Vector3Int startCell = pathTilemap.WorldToCell(spawnPoint.position);
        Vector3Int endCell = pathTilemap.WorldToCell(endPoint.position);
        
        //BFS로 셀 경로 탐색
        var queue = new Queue<Vector3Int>();
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var visited = new HashSet<Vector3Int>();
        
        queue.Enqueue(startCell);
        visited.Add(startCell);
        
        
        //4방향 이웃
        Vector3Int[] dirs = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
        };

        bool found = false;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == endCell)
            {
                found = true;
                break;
            }

            foreach (var d in dirs)
            {
                var next = current + d;
                //길 타일이 있는 셀만 이동 가능
                if (visited.Contains(next))
                    continue;
                if (pathTilemap.GetTile(next) == null)
                    continue;
                
                visited.Add(next);
                queue.Enqueue(next);
                cameFrom[next] = current;
            }
        }

        if (!found)
        {
            Debug.Log("경로 못 찾음");
            return;
        }
        
        //endCell에서 startCell까지 역추적 후 뒤집기
        var pathCells = new List<Vector3Int>();
        var cur = endCell;
        pathCells.Add(cur);
        while (cur != startCell)
        {
            cur = cameFrom[cur];
            pathCells.Add(cur);
        }
        pathCells.Reverse();
        
        //셀 -> 월드 중앙 좌표로 변환
        foreach (var cell in pathCells)
        {
            Vector3 center = pathTilemap.GetCellCenterWorld(cell);
            worldPath.Add(center);
        }
    }
}

