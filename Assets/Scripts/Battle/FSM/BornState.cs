using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BornState : IState
{
    public void Enter(EntityBase entity, params object[] args){
        entity.currentState = AniState.born;
    }
    public void Process(EntityBase entity, params object[] args){
        // 播放出生动画
        entity.SetAction(Constants.ActionBorn);
        TimerService.Instance.AddTimeTask((int tid)=>{
            // 结束出生动画
            entity.SetAction(Constants.ActionDefault);
        }, 500);
    }
    public void Exit(EntityBase entity, params object[] args){
    }
}
