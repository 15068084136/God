using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家实体
/// </summary>
public class PlayerEntity : EntityBase
{
    public PlayerEntity(){
        entityType = EntityType.player;
    }
    // 实体类获得了UI方向盘输入的功能
    public override Vector2 GetCurrnetDir()
    {
        return battleManager.GetCurrnetDir();
    }
    // 覆盖寻找怪物的方向方法
    public override Vector2 CalTargetDir()
    {
        MonsterEntity monster = FindClosedTarget();
        if(monster != null){
            Vector3 target = monster.GetPos();
            Vector3 self = GetPos();
            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;
        }else{
            return Vector2.zero;
        }
    }
    private MonsterEntity FindClosedTarget(){
        List<MonsterEntity> list = battleManager.GetMonsterEntity();
        if(list == null || list.Count == 0){
            return null;
        }
        Vector3 self = GetPos();
        MonsterEntity targetMonster = null;
        float dis = 0;
        for (int i = 0; i < list.Count; i++)
        {   
            // 获得每个附近怪物的距离
            Vector3 targetPos = list[i].GetPos();
            if(i == 0){
                dis = Vector3.Distance(self, targetPos);
                targetMonster = list[i];
            }else{
                float calDis = Vector3.Distance(self, targetPos);
                if(dis > calDis){
                    dis = calDis;
                    targetMonster = list[i];
                }
            }
        }
        return targetMonster;
    }

    public override void SetHpVal(int oldVal, int newVal)
    {
        BattleSys.instance.playerContrlWin.SetHPVal(newVal);
    }
    public override void SetDodge()
    {
        GameRoot.instance.dynamicWin.SetSelDodge();
    }
}
