using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnManager))]
public class SpawnManagerEditor : Editor
{
    private string enemyKey = "enemy_default";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        GUILayout.Space(15);
        EditorGUILayout.LabelField("=== SpawnManager Debug Tools ===", EditorStyles.boldLabel);

        SpawnManager manager = (SpawnManager)target;
        
        //Enemy Key 입력 필드
        enemyKey = EditorGUILayout.TextField("Enemy Key", enemyKey);

        if (GUILayout.Button("Start Spawn Enemy"))
        {
            if (Application.isPlaying)
            {
                manager.StartSpawnEnemyLoop(enemyKey);
                Debug.Log($"[Editor] Start Spawn Enemy: {enemyKey}");
            }
            else
            {
                Debug.Log("Play Mode에서만 Enemy를 스폰할 수 있습니다.");
            }
        }
        
        GUILayout.Space(10);
        
        //Enemy Auto Spawn 정지
        if (GUILayout.Button("Stop Spawn Enemy"))
        {
            if (Application.isPlaying)
            {
                manager.StopSpawnEnemyLoop();
                Debug.Log($"[Editor] Stop Spawn Enemy");
            }
            else
            {
                Debug.Log("Play Mode에서만 스폰을 해제할 수 있습니다.");
            }
        }
        
    }
}
