using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (FindObjectsOfType<T>().Length > 1)
                {
                    Debug.Log($"[Singleton] 중복 인스턴스 발견: {typeof(T)}");
                }

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject($"[Singleton]{typeof(T)}");
                    _instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            
            return _instance;
        }
    }
}
