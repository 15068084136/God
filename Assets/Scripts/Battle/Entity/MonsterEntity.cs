using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物逻辑实体
/// </summary>
public class MonsterEntity : EntityBase
{
    private float checkTime = 2;
    private float checkCountTime = 0;

    private float atkTime = 2; // 攻击间隔
    private float atkCountTime = 0;
    public MonsterData md;

    // 构造函数
    public MonsterEntity(){
        entityType = EntityType.monster;
    }

    // 覆盖掉父类中关于设置战斗数据的方法，原方法是没有根据等级进行计算属性的
    public override void SetBattleProps(BattleProps battleProps)
    {
        int level = md.mLevel;

        BattleProps p = new BattleProps{
            hp = battleProps.hp * level,
            ad = battleProps.ad * level,
            ap = battleProps.ap * level,
            addef = battleProps.addef * level,
            apdef = battleProps.apdef * level,
            dodge = battleProps.dodge * level,
            pierce = battleProps.pierce * level,
            critical = battleProps.critical * level,
        };

        Props = p;
        HP = p.hp;
    }
    public override void TickAiLogic()
    {
        if(!runAI){
            return;
        }
        if(currentState == AniState.idle || currentState == AniState.move){

            if(battleManager.isPauseGame){
                Idle();
                return;
            }

            float delta = Time.deltaTime;
            checkCountTime += delta;
            if(checkCountTime < checkTime){
                return;
            }else{
                // 计算目标方向
                Vector2 dir = CalTargetDir();

                // 判断目标是否在攻击范围内
                if(!InAtkRange()){
                    // 不在，则设置移动方向，并移动
                    SetDir(dir);
                    Move();
                }else{
                    // 在，则停止移动，进行攻击
                    SetDir(Vector2.zero);
                    // 判断攻击间隔
                    atkCountTime += checkCountTime; // 将怪物移动的时间也加入了攻击间隔累加
                    if(atkCountTime > atkTime){
                        // 达到攻击时间，转向并攻击
                        SetAtkRotation(dir);
                        Attack(md.monsterCfg.skillID);
                        atkCountTime = 0;
                    }else{
                        // 未达到攻击事件，Idle等待
                        Idle();
                    }
                }
                checkCountTime = 0;
                // 给检测时间一点随机性
                checkTime = PETools.RandomInt(1, 5) * 1.0f/10;
            }
        }
    }
    bool runAI = true;// AI是否需要找寻玩家
    public override Vector2 CalTargetDir()
    {
        PlayerEntity playerEntity = battleManager.playerEntity;
        if(playerEntity == null || playerEntity.currentState == AniState.die){
            runAI = false;
            return Vector2.zero;
        }else{
            Vector3 target = playerEntity.GetPos();
            Vector3 self = GetPos();
            return new Vector2(target.x - self.x, target.z - self.z);
        }
    }
    private bool InAtkRange(){
        PlayerEntity playerEntity = battleManager.playerEntity;
        if(playerEntity == null || playerEntity.currentState == AniState.die){
            runAI = false;
            return false;
        }else{
            Vector3 target = playerEntity.GetPos();
            Vector3 self = GetPos();
            target.y = 0;
            self.y = 0;
            float dis = Vector3.Distance(target, self);
            if(dis <= md.monsterCfg.atkDis){
                return true;
            }else{
                return false;
            }
        }
    }
    public override bool GetBreakState()
    {
        // 全局可以被中断
        if(md.monsterCfg.isStop){
            if(skillCfg != null){
                // 当前正在释放技能
                // 全局可以被中断的情况下只有施放不被打断的技能才能不被打断
                return skillCfg.isBreak;
            }else{
                return true;
            }
        }else{
            // 全局不可以被打断，就是说明什么时候都不能被打断
            return false;
        }
    }
    public override void SetHpVal(int oldVal, int newVal)
    {
        if(md.monsterCfg.mType == MonsterType.boss){
            BattleSys.instance.playerContrlWin.SetBossHPBarVal(oldVal, newVal, Props.hp);
        }else{
            base.SetHpVal(oldVal, newVal);
        }
    }
}
