using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 逻辑实体基类
/// </summary>
public class EntityBase
{
    public EntityType entityType = EntityType.none;
    public EntityState entityState = EntityState.none;
    public AniState currentState;
    public BattleManager battleManager = null;
    public StateManager stateManager = null;
    public SkillManager skillManager = null;
    protected Controller controller;
    private BattleProps props;
    public BattleProps Props{
        get{
            return props;
        }
        // 只有在子类中才能访问，从而保护了数据安全性
        protected set{
            props = value;
        }
    }
    private int hp;
    public int HP{
        get{
            return hp;
        }set{
            // 通知UI
            PECommon.Log(hp + " to " + value);
            SetHpVal(hp, value);
            hp = value;
        }
    }
    private string name;
    public string Name{
        get{
            return name;
        }set{
            name = value;
        }
    }

    public Queue<int> comboQue = new Queue<int>();
    public int nextSkillID;

    public bool canControl = true;// 是否可以控制玩家移动
    public bool canRlsSkill = true;// 是否可以施放技能

    public SkillCfg skillCfg;

    // 技能位移的回调ID
    public List<int> skillMoveCBList = new List<int>();
    // 技能伤害计算回调ID
    public List<int> skillActionCBList = new List<int>();

    public virtual void SetBattleProps(BattleProps battleProps){
        // 战斗一开始先设置当前血量为属性血量
        HP = battleProps.hp;
        Props = battleProps;
    }
    public void Born(){
        stateManager.ChangeState(this, AniState.born, null);
    }
    public void Move(){
        stateManager.ChangeState(this, AniState.move, null);
    }
    public void Idle(){
        stateManager.ChangeState(this, AniState.idle, null);
    }
    public void Attack(int skillId){
        stateManager.ChangeState(this, AniState.attack, skillId);
    }
    public void Hit(){
        stateManager.ChangeState(this, AniState.hit, null);
    }
    public void Die(){
        stateManager.ChangeState(this, AniState.die, null);
    }
    // 通过逻辑实体，影响表现实体
    public virtual void SetBlend(float blend){
        if(controller != null){
            controller.SetBlend(blend);
        }
    }
    public virtual void SetDir(Vector2 dir){
        if(controller != null){
            controller.Dir = dir;
        }
    }
    public virtual void SetAction(int act){
        if(controller != null){
            controller.SetAction(act);
        }
    }
    public virtual void SkillAttack(int skillId){
        skillManager.SkillAttack(this, skillId);
    }
    public virtual void SetFx(string name, float destroy){
        if(controller != null){
            controller.SetFx(name, destroy);
        }
    }
    public virtual void SetSkillMoveState(bool move, float speed = 0f){
        if(controller != null){
            controller.SetSkillMoveState(move, speed);
        }
    }
    public virtual void SetAtkRotation(Vector2 dir, bool offset = false){
        if(controller != null){
            if(offset){
                // 需要偏离
                controller.SetAtkRotationCam(dir);
            }else{
                controller.SetAtkRotationLocal(dir);
            }
        }
    }
    #region 战斗信息显示
    public virtual void SetDodge(){
        if(controller != null){
            GameRoot.instance.dynamicWin.SetDodge(Name);
        }
    }
    public virtual void SetCritical(int critical){
        if(controller != null){
            GameRoot.instance.dynamicWin.SetCritical(Name, critical);
        }
    }
    public virtual void SetHurt(int hurt){
        if(controller != null){
            GameRoot.instance.dynamicWin.SetHurt(Name, hurt);
        }
    }
    public virtual void SetHpVal(int oldVal, int newVal){
        if(controller != null){
            GameRoot.instance.dynamicWin.SetHpVal(Name, oldVal, newVal);
        }
    }
    #endregion
    // 获得当前方向向量的虚函数
    public virtual Vector2 GetCurrnetDir(){
        return Vector2.zero;
    }
    // 获得当前实体的位置
    public virtual Vector3 GetPos(){
        return controller.transform.position;
    }
    // 获得当前实体的Transform
    public virtual Transform GetTransform(){
        return controller.transform;
    }
    public CharacterController GetCC(){
        return controller.GetComponent<CharacterController>();
    }
    // 获得动画列表
    public AnimationClip[] GetAniClips(){
        if(controller != null){
            return controller.animator.runtimeAnimatorController.animationClips;
        }
        return null;
    }
    // 获得AudioSource
    public AudioSource GetAudioSource(){
        return controller.GetComponent<AudioSource>();
    }
    public void SetCtrl(Controller ctrl){
        controller = ctrl;
    }
    public void SetActive(bool active = true){
        if(controller != null){
            controller.gameObject.SetActive(active);
        }
    }
    public void ExitCurrentSkill(){
        canControl = true;
        if(skillCfg != null){
            if(!skillCfg.isBreak){
                // 技能结束取消霸体
                entityState = EntityState.none;
            }
            if(skillCfg.isCombo){
                if(comboQue.Count > 0){
                    nextSkillID = comboQue.Dequeue();
                }else{
                    nextSkillID = 0;
                }
            }
        }
        skillCfg = null;// 将当前技能的Cfg配置数据设置为空，说明这次技能打完了
        SetAction(Constants.ActionDefault);
    }

    // 找到目标的方向
    public virtual Vector2 CalTargetDir(){
        return Vector2.zero;
    }

    // 怪物的AI逻辑
    public virtual void TickAiLogic(){

    }
    // 移除回调函数列表中的回调函数
    public void RemoveMoveCB(int tid){
        int index = -1;
        for (int i = 0; i < skillMoveCBList.Count; i++)
        {
            if(skillMoveCBList[i] == tid){
                // 找到了对应的回调函数
                index = i;
                break;
            }
        }
        if(index != -1){
            // 说明找到了
            skillMoveCBList.RemoveAt(index);
        }
    }

    public void RemoveActCB(int tid){
        int index = -1;
        for (int i = 0; i < skillActionCBList.Count; i++)
        {
            if(skillActionCBList[i] == tid){
                // 找到了对应的回调函数
                index = i;
                break;
            }
        }
        if(index != -1){
            // 说明找到了
            skillActionCBList.RemoveAt(index);
        }
    }
    // 获得当前是否可以被打断状态（isStop）
    public virtual bool GetBreakState(){
        return true;
    }
    // 获取当前的controller
    public Controller GetController(){
        return controller;
    }
}
