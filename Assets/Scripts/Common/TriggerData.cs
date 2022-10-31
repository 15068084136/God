using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerData : MonoBehaviour
{
    public MapManager mapManager;
    public int waveIndex;// 生成第几波怪物
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Player"){
            if(mapManager != null){
                mapManager.TriggerMonsterBorn(this, waveIndex);
            }
        }
    }
}
