using System.Security.Claims;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongCfg : BaseData<StrongCfg>{
    public int pos;
    public int startLv;
    public int addHp;
    public int addHurt;
    public int addDef;
    public int minLv;
    public int coin;
    public int crystal;
}

public class AutoGuideCfg : BaseData<AutoGuideCfg>{
    public int npcID;
    public string dialogArr;
    public int actID;
    public int coin;
    public int exp;
}

public class MapCfg : BaseData<MapCfg>{
    public string mapName;
    public string sceneName;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
    public int power;// 需要消耗的体力值
    public List<MonsterData> monsterList;
    public int coin;
    public int exp;
    public int crystal;
}

// 不会变化的数据
public class TaskCfg : BaseData<TaskCfg>{
    public string taskName;
    public int count;
    public int exp;
    public int coin;
}

// 会变化的逻辑数据
public class TaskData : BaseData<TaskData>{
    // 任务进度
    public int prgs;
    //  任务奖励是否已经被领取
    public bool isToken;
}

public class SkillCfg : BaseData<SkillCfg>{
    public string skillName;
    public int skillTime;
    public int aniAction;
    public string fxName;
    public int cdTime;
    public bool isCombo;
    public bool isCollider;// 是否检测碰撞体
    public bool isBreak;// 技能是否可以被打断
    public DamageType dmgType;
    public List<int> skillMoveLst;// 指向SkillMove配置文件
    public List<int> skillActionLst;// 指向SkillAction配置文件
    public List<int> skillDamageLst;
}

public class SkillMoveCfg : BaseData<SkillMoveCfg>{
    public int moveTime;
    public float moveDis;
    public int delayTime;
}

public class SkillActionCfg : BaseData<SkillActionCfg>{
    public int delayTime;
    public float radius;// 伤害计算范围
    public int angle;// 伤害有效角度
}

public class MonsterCfg : BaseData<MonsterCfg>{
    public string monsterName;
    public MonsterType mType;// 怪物类型：1是普通怪物 2是Boss怪物
    public bool isStop;// 是否可以被中断
    public string resPath;
    public int skillID;
    public float atkDis;
    public BattleProps battleProps;
}

public class MonsterData : BaseData<MonsterData>{
    public int mWave; // 怪物的批次
    public int mIndex; // 怪物在当前批次的序号
    public MonsterCfg monsterCfg; // 当前怪物的配置文件
    public Vector3 mBornPos;
    public Vector3 mBornRote;
    public int mLevel;
}

/// <summary>
/// 所有配置数据类的基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseData<T>
{
    public int ID;
}

// 专门计算战斗中的属性数据
public class BattleProps{
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int dodge;
    public int pierce;
    public int critical;
}
