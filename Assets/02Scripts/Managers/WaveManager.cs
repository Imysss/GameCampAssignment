using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveInfo
{
    public string enemyKey;
    public int count = 5;
    public float spawnInterval = 0.5f;
}

public class WaveManager : Singleton<WaveManager>
{
    [Header("Wave Settings")] 
    [SerializeField] private List<WaveInfo> waves;

    private int _currentWave = 0;

    private int _aliveEnemyCount = 0;
    private bool _stageRunning = false;

    private void OnEnable()
    {
        EnemyController.OnEnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        EnemyController.OnEnemyKilled -= HandleEnemyKilled;
    }
    
    //Stage START!!!!!!!!!!!!! ㅜㅜ
    public void StartStage()
    {
        if (_stageRunning)
            return;

        _stageRunning = true;
        _currentWave = 0;

        StartCoroutine(CoStageRoutine());
    }

    private IEnumerator CoStageRoutine()
    {
        Debug.Log("Stage Start!");

        while (_currentWave < waves.Count)
        {
            yield return StartCoroutine(CoWaveRoutine(_currentWave));
            _currentWave++;
        }

        StageClear();
    }
    
    //Wave Routine
    public void StartWave(int idx)
    {
        if (waves.Count <= idx)
        {
            Debug.Log("해당 Wave는 존재하지 않습니다");
            return;
        }
        StartCoroutine(CoWaveRoutine(idx));
    }
    
    private IEnumerator CoWaveRoutine(int waveIdx)
    {
        WaveInfo wave = waves[waveIdx];
        
        Debug.Log($"Wave {waveIdx+1} 시작");

        _aliveEnemyCount = wave.count;

        for (int i = 0; i < wave.count; i++)
        {
            SpawnManager.Instance.SpawnEnemy(wave.enemyKey);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
        
        //Enemy 죽을 때마다 _aliveEnemyCount 감소 -> 0 되면 웨이브 종료
        yield return new WaitUntil(() => _aliveEnemyCount <= 0);

        Debug.Log($"Wave {waveIdx+1} 클리어");
    }
    
    //Enemy Death Event
    private void HandleEnemyKilled(EnemyController ec)
    {
        _aliveEnemyCount--;
    }
    
    //Stage Clear
    private void StageClear()
    {
        _stageRunning = false;
        Debug.Log("Stage Clear");
        //TODO: Clear UI 띄우기
    }

    public void StageFail()
    {
        _stageRunning = false;
        Debug.Log("Stage Fail");
        
        StopAllCoroutines();
        //TODO: Fail UI 띄우기
    }
}
