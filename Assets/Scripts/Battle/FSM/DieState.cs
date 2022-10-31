using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : IState
{
    public void Enter(EntityBase entity, params object[] args){
        entity.currentState = AniState.die;
    }
    public void Process(EntityBase entity, params object[] args){
        entity.SetAction(Constants.ActionDie);
        if(entity.entityType == EntityType.monster){
            // 死亡时将怪物的碰撞体设置为空
            entity.GetCC().enabled = false;
            TimerService.Instance.AddTimeTask((int tid)=>{
            // 目的是让角色死亡时，不要让角色身上的AudioListner消失
            entity.SetActive(false);
            }, Constants.DieAniLength);
        }
    }
    public void Exit(EntityBase entity, params object[] args){
    }
}
