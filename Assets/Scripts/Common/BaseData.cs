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

#region 物品BaseData
/// <summary>
/// 物品基类
/// </summary>
public class Item
{
    // 外部可以访问，但修改只能内部修改
    public int ID { get; set; }
    public string Name { get; set; }
    public ItemType Type { get; set; }
    public ItemQuality Quality { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }
    public int BuyPrice { get; set; }
    public int SellPrice { get; set; }
    public string Sprite { get; set; }

    public Item()
    {
        this.ID = -1;
    }

    public Item(int id, string name, ItemType type, ItemQuality quality, string des, int capacity, int buyPrice, int sellPrice,string sprite)
    {
        this.ID = id;
        this.Name = name;
        this.Type = type;
        this.Quality = quality;
        this.Description = des;
        this.Capacity = capacity;
        this.BuyPrice = buyPrice;
        this.SellPrice = sellPrice;
        this.Sprite = sprite;
    }

    /// <summary>
    /// 物品类型
    /// </summary>
    public enum ItemType
    {
        Consumable,
        Equipment,
        Weapon,
        Material
    }
    /// <summary>
    /// 品质
    /// </summary>
    public enum ItemQuality
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Artifact
    }

    /// <summary> 
    /// 得到提示面板应该显示什么样的内容
    /// </summary>
    /// <returns></returns>
    public virtual string GetToolTipText()
    {
        string color = "";
        switch (Quality)
        {
            case ItemQuality.Common:
                color = "white";
                break;
            case ItemQuality.Uncommon:
                color = "lime";
                break;
            case ItemQuality.Rare:
                color = "navy";
                break;
            case ItemQuality.Epic:
                color = "magenta";
                break;
            case ItemQuality.Legendary:
                color = "orange";
                break;
            case ItemQuality.Artifact:
                color = "red";
                break;
        }
        string text = string.Format("<color={4}>{0}</color>\n<size=30><color=green>购买价格：{1} 出售价格：{2}</color></size>\n<color=yellow><size=30>{3}</size></color>", Name, BuyPrice, SellPrice, Description, color);
        return text;
    }
}

/// <summary>
/// 消耗品物品类
/// </summary>
public class Consumable : Item
{
    public int HP { get; set; }
    public int MP { get; set; }
    //调用父类的构造方法
    public Consumable(int id, string name, ItemType type, ItemQuality quality, string des, int capacity, int buyPrice, int sellPrice, string sprite, int hp, int mp) : base(id, name, type, quality, des, capacity, buyPrice, sellPrice, sprite)
    {
        this.HP = hp;
        this.MP = mp;
    }

    // 重写显示信息
    public override string GetToolTipText()
    {
        // 获得父类的显示信息
        string text = base.GetToolTipText();

        // 第一行是原来的提示信息
        string newText = string.Format("{0}\n\n<color=blue>加血：{1}\n加蓝：{2}</color>", text, HP, MP);

        return newText;
    }
}

/// <summary>
/// 武器类物品
/// </summary>
public class Equipment : Item
{
    /// <summary>
    /// 力量
    /// </summary>
    public int Strength { get; set; }
    /// <summary>
    /// 智力
    /// </summary>
    public int Intellect { get; set; }
    /// <summary>
    /// 敏捷
    /// </summary>
    public int Agility { get; set; }
    /// <summary>
    /// 体力
    /// </summary>
    public int Stamina { get; set; }
    /// <summary>
    /// 装备类型
    /// </summary>
    public EquipmentType EquipType { get; set; }

    public Equipment(int id, string name, ItemType type, ItemQuality quality, string des, int capacity, int buyPrice, int sellPrice,string sprite,
        int strength,int intellect,int agility,int stamina,EquipmentType equipType)
        : base(id, name, type, quality, des, capacity, buyPrice, sellPrice,sprite)
    {
        this.Strength = strength;
        this.Intellect = intellect;
        this.Agility = agility;
        this.Stamina = stamina;
        this.EquipType = equipType;
    }

    public enum EquipmentType
    {
        None,
        Head,
        Neck,
        Chest,
        Ring,
        Leg,
        Bracer,
        Boots,
        Shoulder,
        Belt,
        OffHand
    }

    public override string GetToolTipText()
    {
        string text = base.GetToolTipText();

        string equipTypeText = "";
        switch (EquipType)
	{
		case EquipmentType.Head:
                equipTypeText="头部";
         break;
        case EquipmentType.Neck:
                equipTypeText="脖子";
         break;
        case EquipmentType.Chest:
                equipTypeText="胸部";
         break;
        case EquipmentType.Ring:
                equipTypeText="戒指";
         break;
        case EquipmentType.Leg:
                equipTypeText="腿部";
         break;
        case EquipmentType.Bracer:
                equipTypeText="护腕";
         break;
        case EquipmentType.Boots:
                equipTypeText="靴子";
         break;
        case EquipmentType.Shoulder:
                equipTypeText="护肩";
         break;
        case EquipmentType.Belt:
                equipTypeText = "腰带";
         break;
        case EquipmentType.OffHand:
                equipTypeText="副手";
         break;
	}

        string newText = string.Format("{0}\n\n<color=blue>装备类型：{1}\n力量：{2}\n智力：{3}\n敏捷：{4}\n体力：{5}</color>", text,equipTypeText,Strength,Intellect,Agility,Stamina);

        return newText;
    }
}

/// <summary>
/// 材料类物品
/// </summary>
public class Material : Item
{
    public Material(int id, string name, ItemType type, ItemQuality quality, string des, int capacity, int buyPrice, int sellPrice,string sprite)
        : base(id, name, type, quality, des, capacity, buyPrice, sellPrice,sprite)
    {
    }
}

/// <summary>
/// 武器类物品
/// </summary>
public class Weapon : Item
{
    public int Damage { get; set; }

    public WeaponType WpType { get; set; }

    public Weapon(int id, string name, ItemType type, ItemQuality quality, string des, int capacity, int buyPrice, int sellPrice,string sprite,
       int damage,WeaponType wpType)
        : base(id, name, type, quality, des, capacity, buyPrice, sellPrice,sprite)
    {
        this.Damage = damage;
        this.WpType = wpType;
    }

    public enum WeaponType
    {
        None,
        OffHand,
        MainHand
    }

    public override string GetToolTipText()
    {
        string text = base.GetToolTipText();

        string wpTypeText = "";

        switch (WpType)
        {
            case WeaponType.OffHand:
                wpTypeText = "副手";
                break;
            case WeaponType.MainHand:
                wpTypeText = "主手";
                break;
        }

        string newText = string.Format("{0}\n\n<color=blue>武器类型：{1}\n攻击力：{2}</color>", text, wpTypeText, Damage);

        return newText;
    }
}
#endregion
