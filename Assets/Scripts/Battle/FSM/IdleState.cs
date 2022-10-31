using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    public void Enter(EntityBase entity, params object[] args){
        entity.currentState = AniState.idle;
    }
    public void Process(EntityBase entity, params object[] args){
        // 如果普攻连招没有完成
        if(entity.nextSkillID != 0){
            entity.Attack(entity.nextSkillID);
        }else{
            if(entity.entityType == EntityType.player){
                // 在Idle状态下可以施放技能
                entity.canRlsSkill = true;
            }
            // 如果是怪物就是Zero，主要解决的是施放完技能无法移动的问题
            if(entity.GetCurrnetDir() != Vector2.zero){
                entity.Move();
                entity.SetDir(entity.GetCurrnetDir());
            }else{
                // 如果是怪物，则播放Idle动画
                entity.SetBlend(Constants.BlendIdle);
            }
        }
    }
    public void Exit(EntityBase entity, params object[] args){
    }
}
