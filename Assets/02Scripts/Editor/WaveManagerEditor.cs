using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor
{
    private int testWaveIdx = 0;

    public override void OnInspectorGUI()
    {
        //기본 인스펙터 그리기
        base.OnInspectorGUI();
        
        WaveManager wm = (WaveManager)target;
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== Wave Test Tools ===", EditorStyles.boldLabel);
        
        //=== 스테이지 시작 버튼 ===
        if (GUILayout.Button("Start Stage"))
        {
            if (Application.isPlaying)
            {
                wm.StartStage();
            }
            else
            {
                Debug.Log("[WaveManagerEditor] Play Mode에서만 실행 가능합니다");
            }
        }
        
        //=== 웨이브 시작 버튼 ===
        //Wave Index 입력
        testWaveIdx = EditorGUILayout.IntField("Test Wave Index", testWaveIdx);
        
        //실행 버튼
        if (GUILayout.Button("Start Selected Wave"))
        {
            if (Application.isPlaying)
            {
                wm.StartWave(testWaveIdx);
            }
            else
            {
                Debug.Log("[WaveManagerEditor] Play Mode에서만 실행 가능합니다");
            }
        }
    }
}
