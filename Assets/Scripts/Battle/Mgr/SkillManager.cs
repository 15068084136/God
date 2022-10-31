using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 技能管理器
/// </summary>
public class SkillManager : MonoBehaviour
{
    ResService resService = null;
    TimerService timerService = null;
    public void Init(){
        resService = ResService.instance;
        timerService = TimerService.Instance;
    }
    public void SkillAttack(EntityBase entity, int skillId){
        entity.skillMoveCBList.Clear();
        entity.skillActionCBList.Clear();

        AttackDamage(entity, skillId);
        AttackEffect(entity, skillId);
    }

    // 技能伤害计算
    public void AttackDamage(EntityBase entity, int skillId){
        SkillCfg skillCfg = resService.GetSkillCfg(skillId);
        List<int> actionList = skillCfg.skillActionLst;
        int sum = 0;
        for (int i = 0; i < actionList.Count; i++)
        {
            SkillActionCfg actionCfg = resService.GetSkillActionCfg(actionList[i]);
            sum += actionCfg.delayTime;
            int index = i;
            if(sum > 0){
                int actCBID = timerService.AddTimeTask((int tid)=>{
                    if(entity != null){
                        SkillActiion(entity, skillCfg, index);
                        entity.RemoveActCB(tid);
                    }
                }, sum);
                entity.skillActionCBList.Add(actCBID);
            }else{
                // 瞬时技能
                SkillActiion(entity, skillCfg, index);
            }
        }
    }

    public void SkillActiion(EntityBase entity, SkillCfg skillCfg, int index){
        SkillActionCfg skillActionCfg = resService.GetSkillActionCfg(skillCfg.skillActionLst[index]);

        if(!skillCfg.isCollider){
            // 忽略碰撞体之间的交互
            Physics.IgnoreLayerCollision(6, 7);
            timerService.AddTimeTask((int tid)=>{
                // 重新建立交互
                Physics.IgnoreLayerCollision(6, 7, false);
            }, skillCfg.skillTime);
        }

        int damage = skillCfg.skillDamageLst[index];

        if(entity.entityType == EntityType.monster){
            PlayerEntity target = entity.battleManager.playerEntity;
            if(target == null){
                return;
            }

            // 判断距离和角度
            if(InRange(entity.GetPos(), target.GetPos(), skillActionCfg.radius)
            && InAngel(entity.GetTransform(), target.GetPos(), skillActionCfg.angle)){
                // 计算伤害
                CalDamage(entity, target, skillCfg, damage);
            }
        }else if(entity.entityType == EntityType.player){ 
            // 获取场景里所有的怪物实体，遍历运算
            List<MonsterEntity> monsterList = entity.battleManager.GetMonsterEntity();
            for (int i = 0; i < monsterList.Count; i++){
                MonsterEntity me = monsterList[i];
                // 判断距离和角度
                if(InRange(entity.GetPos(), me.GetPos(), skillActionCfg.radius)
                && InAngel(entity.GetTransform(), me.GetPos(), skillActionCfg.angle)){
                    // 计算伤害
                    CalDamage(entity, me, skillCfg, damage);
                }
            }
        }

    }

    System.Random rd = new System.Random();
    private void CalDamage(EntityBase entity, EntityBase target, SkillCfg skillCfg, int damage){
        int dmgSum = damage; // 计算伤害总和
        if(skillCfg.dmgType == DamageType.ad){
            // 计算闪避
            int dodgeNum = PETools.RandomInt(1, 100, rd);
            if(dodgeNum < target.Props.dodge){
                // 闪避成功
                PECommon.Log("闪避Rate:" + dodgeNum + "/" + target.Props.dodge);
                target.SetDodge();
                return;
            }
            // 计算属性加成
            dmgSum += entity.Props.ad;
            // 计算暴击
            int criticalNum = PETools.RandomInt(1, 100, rd);
            if(criticalNum <= entity.Props.critical){
                // 暴击伤害在1倍到2倍之间
                float criticalRate = 1 + (PETools.RandomInt(1, 100, rd)/100.0f);
                dmgSum = (int)criticalRate * dmgSum;
                PECommon.Log("暴击Rate:" + criticalNum + "/" + target.Props.critical);
                target.SetCritical(dmgSum);
            }

            // 计算穿甲
            int adddef = (int)((1 - entity.Props.pierce/100.0f) * target.Props.addef);
            dmgSum -= adddef;
        }else if(skillCfg.dmgType == DamageType.ap){
            // 计算属性加成
            dmgSum += entity.Props.ap;
            // 计算魔法抗性
            dmgSum -= target.Props.apdef;
        }else{}

        // 最终伤害
        if(dmgSum < 0){
            dmgSum = 0;
            return;
        }
        target.SetHurt(dmgSum);

        if(target.HP < dmgSum){
            // 目标死亡
            target.HP = 0;
            target.Die();

            if(target.entityType == EntityType.monster){
                // 清空怪物数据，并销毁血条
                target.battleManager.RemoveMonster(target.Name);
            }else if(target.entityType == EntityType.player){
                target.battleManager.EndBattle(false, 0);
                target.battleManager.playerEntity = null;
            }

            
        }else{
            target.HP -= dmgSum;
            //if(entity.entityState == EntityState.none && target.GetBreakState()){
                // 播放受击动画
            if(entity.entityState != EntityState.baTiState){
                target.Hit();
            }
            //}
        }
    }

    private bool InRange(Vector3 from, Vector3 to, float range){
        float dis = Vector3.Distance(from, to);
        if(dis <= range){
            return true;
        }
        return false;
    }
    private bool InAngel(Transform trans, Vector3 to, float angle){
        if(angle == 360){
            return true;
        }else{
            Vector3 start = trans.forward;// 玩家的正前方
            Vector3 dir = (to - trans.position).normalized;// 玩家和敌人的朝向向量
            float ang = Vector3.Angle(start, dir);// 朝向向量与正前方的夹角

            if(ang <= angle/2){
                return true;
            }
            return false;
        }
    }

    #region 技能效果表现
    // 技能效果表现
    public void AttackEffect(EntityBase entity, int skillId){
        SkillCfg skillCfg = resService.GetSkillCfg(skillId);

        // 只有player才能智能锁定怪物
        if(entity.entityType == EntityType.player){
            if(entity.GetCurrnetDir() == Vector2.zero){
                // 搜索最近的怪物
                Vector2 dir = entity.CalTargetDir();
                if(dir != Vector2.zero){
                    // 说明找到了最近的怪物，不需要摄像机偏移
                    entity.SetAtkRotation(dir);
                }
            }else{
                // 在攻击过程中有方向输入,且需要摄像机偏移
                entity.SetAtkRotation(entity.GetCurrnetDir(), true);
            }
        }

        // 设置人物技能动画
        entity.SetAction(skillCfg.aniAction);
        // 设置技能特效
        entity.SetFx(skillCfg.fxName, skillCfg.skillTime);

        CalSkillMove(entity, skillCfg);

        entity.canControl = false;
        entity.SetDir(Vector2.zero);

        if(!skillCfg.isBreak){
            entity.entityState = EntityState.baTiState;
        }

        // 技能施放结束进入Idle状态
        timerService.AddTimeTask((int tid)=>{
            entity.Idle();
        }, skillCfg.skillTime);
    }

    // 计算人物技能施放距离
    public void CalSkillMove(EntityBase entity, SkillCfg skillCfg){
        List<int> skillMoveLst = skillCfg.skillMoveLst;
        int sum = 0;// 总技能时间
        // 循环单个技能内每个动作
        for (int i = 0; i < skillMoveLst.Count; i++)
        {
            // 计算技能动画移动速度
            SkillMoveCfg skillMoveCfg = resService.GetSkillMoveCfg(skillCfg.skillMoveLst[i]);
            float speed = skillMoveCfg.moveDis/(skillMoveCfg.moveTime/1000f);
            sum += skillMoveCfg.delayTime;
            // 延迟施法(第二次施法是在延迟了680ms后进行的)
            if(sum > 0){
                int moveCBID = timerService.AddTimeTask((int tid)=>{
                    entity.SetSkillMoveState(true, speed);
                    // 运行结束需要在回调函数列表中删除
                    entity.RemoveMoveCB(tid);
                }, sum);
                entity.skillMoveCBList.Add(moveCBID);
            }else{
                // 设置逻辑实体的技能移动数据
                entity.SetSkillMoveState(true, speed);
            }
            sum += skillMoveCfg.moveTime;
            // 施放结束后使得move变为false
            int stopCBID = timerService.AddTimeTask((int tid)=>{
                entity.SetSkillMoveState(false);
                entity.RemoveMoveCB(tid);
            }, sum);
            entity.skillMoveCBList.Add(stopCBID);
        }
    }
    #endregion
}
