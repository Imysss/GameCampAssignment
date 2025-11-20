using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourceManager))]
public class ResourceManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(15);
        EditorGUILayout.LabelField("=== ResourceManager Debug ===", EditorStyles.boldLabel);
        
        ResourceManager manager = (ResourceManager)target;

        if (GUILayout.Button("Start Preload"))
        {
            if (Application.isPlaying)
            {
                manager.Preload();
                Debug.Log("[Editor] ResourceManager Preload 실행");
            }
            else
            {
                Debug.Log("Play Mode에서만 Preload 할 수 있습니다.");
            }
        }
    }
}
