using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAutoFit : MonoBehaviour
{
    public SpriteRenderer mapRenderer;
    private Camera camera;

    private int lastW, lastH;

    private void Awake()
    {
        camera = Camera.main;
        FitCamera();
    }

    private void Update()
    {
        //해상도 변경 감지 (Simulator에서도 작동)
        if (Screen.width != lastW || Screen.height != lastH)
        {
            lastW = Screen.width;
            lastH = Screen.height;
            FitCamera();
        }
    }

    private void FitCamera()
    {  
        Bounds bounds = mapRenderer.bounds;

        float mapWidth = bounds.size.x;
        float mapHeight = bounds.size.y;
        float aspect = (float)Screen.width / Screen.height;

        float sizeByHeight = mapHeight / 2f;
        float sizeByWidth = mapWidth / 2f;
        
        camera.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
        
        //카메라 중앙을 맵 중앙에 맞추기
        camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, camera.transform.position.z);

    }
}
