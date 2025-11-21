using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CommanderController Commander;
    public void OnCommanderDead()
    {
        //Enemy 스폰 중지
        SpawnManager.Instance.StopSpawnEnemyLoop();
        
        //UI GameOver 창 호출
        
        //게임 정지
    }
}
