using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    public void Enter(EntityBase entity, params object[] args){
        entity.currentState = AniState.attack;
        entity.skillCfg = ResService.instance.GetSkillCfg((int)args[0]);
    }
    public void Process(EntityBase entity, params object[] args){
        if(entity.entityType == EntityType.player){
            entity.canRlsSkill = false;
        }

        entity.SkillAttack((int)(args[0]));
    }
    public void Exit(EntityBase entity, params object[] args){
        entity.ExitCurrentSkill();
    }
}
