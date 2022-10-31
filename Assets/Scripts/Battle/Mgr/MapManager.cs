using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 地图管理器
/// </summary>
public class MapManager : MonoBehaviour
{
    private int waveIndex = 1;// 默认第一批怪物
    private BattleManager battleManager;
    public TriggerData[] triggerDatass;
    public void Init(BattleManager battleManager){
        this.battleManager = battleManager;

        // 实例化生成第一批怪物
        battleManager.LoadMonsterByWaveID(waveIndex);
    }
    public void TriggerMonsterBorn(TriggerData triggerData, int waveIndex){
        if(battleManager != null){
            BoxCollider box = triggerData.GetComponent<BoxCollider>();
            box.isTrigger = false;

            battleManager.LoadMonsterByWaveID(waveIndex);
            battleManager.ActiveMonster();// 激活当前批次的怪物
            battleManager.triggerCheck = true;
        }
    }

    public bool SetDoorIsTrigger(){
        waveIndex += 1;
        for (int i = 0; i < triggerDatass.Length; i++)
        {
            if(triggerDatass[i].waveIndex == waveIndex){
                BoxCollider box = triggerDatass[i].GetComponent<BoxCollider>();
                box.isTrigger = true;
                return true;
            }
        }
        return false;
    }
}
