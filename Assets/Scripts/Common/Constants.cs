using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TxtColor{
    red,
    blue,
    green,
    yellow
}
public enum DamageType{
    none,
    ad = 1,
    ap = 2
}
public enum EntityType{
    none,
    player,
    monster
}
public enum EntityState{
    none,
    baTiState // 霸体状态
}
public enum MonsterType{
    none,
    normal = 1,
    boss = 2
}

/// <summary>
/// 常量
/// </summary>
public class Constants : MonoBehaviour
{
    #region 渲染文字颜色
    // 颜色码
    public const string colorRed = "<color=#FF000FF>";
    public const string colorGreen = "<color=#00FF00FF>";
    public const string colorBlue = "<color=#00B4FFFF>";
    public const string colorYellow = "<color=#FFFF00FF>";
    public const string colorEnd = "</color>";

    // 渲染文字颜色
    public static string ColorText(string str, TxtColor color){
        string result = "";
        switch(color){
            case TxtColor.red:
                result = colorRed + str + colorEnd;
                break;
            case TxtColor.green:
                result = colorGreen + str + colorEnd;
                break;
            case TxtColor.blue:
                result = colorBlue + str + colorEnd;
                break;
            case TxtColor.yellow:
                result = colorYellow + str + colorEnd;
                break;
        }
        return result;
    }
    #endregion

    // 场景名称
    public const string SceneLogin = "SceneLogin";
    public const string SceneMainCity = "SceneMainCity";
    public const int MainCityMapID = 10000;

    #region 音乐与音效
    // 音效名称
    public const string LoginBGM = "bgLogin";
    public const string MainCityBGM = "bgMainCity";
    public const string HuangYeBGM = "bgHuangYe";
    public const string UIOpenPage = "uiOpenPage";

    // 登录按钮音效
    public const string UILoginButtonClick = "uiLoginBtn";

    // 普通UI点击音效
    public const string UIClickButton = "uiClickBtn";
    public const string UIExtenBtn = "uiExtenBtn";
    public const string fbItem = "fbitem";
    public const string FBLose = "fblose";
    public const string FBLogoEnter = "fbwin";
    #endregion

    // 遥感操作标准距离
    public const int ScreenDragLength = 90;

    // 移动速度
    public const int PlayerMoveSpeed = 8;
    public const int MonsterMoveSpeed = 3;

    // 运动平滑
    public const float AcceleSpeed = 5f;
    public const float AcceleHpSpeed = 0.3f;

    #region 强化图片
    public const string arrowBg = "ResImages/btnstrong";
    public const string platBg = "ResImages/charbg3";
    
    public const string touKuiItem = "ResImages/toukui";
    public const string bodyItem = "ResImages/body";
    public const string yaobuItem = "ResImages/yaobu";
    public const string handItem = "ResImages/hand";
    public const string legItem = "ResImages/leg";
    public const string footItem = "ResImages/foot";

    // 灰色星星
    public const string grayStar = "ResImages/star1";
    // 星星
    public const string star = "ResImages/star2";
    #endregion

    #region Chat
    public const string chatTypeBtn1 = "Resimages/btntype1";
    public const string chatTypeBtn2 = "Resimages/btntype2";
    #endregion

    #region TaskReward
    public const string TaskItemPrefeb = "Prefeb/TaskBg";
    #endregion

    #region 玩家或者Monster
    public const string playerPrefebPath = "PrefabPlayer/AssassinCity";
    public const string BattleplayerPrefebPath = "PrefabPlayer/AssassinBattle";
    public const int BlendIdle = 0;
    public const int BlendMove = 1;
    public const int ActionDefault = -1;
    public const int ActionBorn = 0;
    public const int ActionDie = 100;
    public const int ActionHit = 101;
    public const string hpItemPath = "Prefeb/HpItem";
    public const int comboSpace = 500;
    public const string assassinHit = "assassin_Hit";
    public const int DieAniLength = 5000;
    #endregion

    #region 配置文件路径
    public const string RandomNameCfgPath = "ResCfgs/rdname";
    public const string MapCfgPath = "ResCfgs/map";
    public const string GuideCfgPath = "ResCfgs/guide";
    public const string strongPath = "ResCfgs/strong";
    public const string taskRewardCfgPath = "ResCfgs/taskreward";
    public const string skillCfgPath = "ResCfgs/skill";
    public const string skillMoveCfgPath = "ResCfgs/skillmove";
    public const string monsterCfgPath = "ResCfgs/monster";
    public const string skillActionCfgPath = "ResCfgs/skillaction";
    #endregion

    #region 任务图标
    public const string taskHead = "ResImages/task";
    public const string wiseManHead = "ResImages/wiseman";
    public const string generalHead = "ResImages/general";
    public const string artisanHead = "ResImages/artisan";
    public const string traderHead = "ResImages/trader";
    // NPC序号
    public const int wiseManNpc = 0;
    public const int generalNpc = 1;
    public const int artisanNpc = 2;
    public const int traderNpc = 3;
    // NPC和玩家的人物像
    public const string selfIconPath = "ResImages/assassin";
    public const string guideIconPath = "ResImages/npcguide";
    public const string wiseManIconPath = "ResImages/npc0";
    public const string generalIconPath = "ResImages/npc1";
    public const string artisanIconPath = "ResImages/npc2";
    public const string traderIconPath = "ResImages/npc3";

    #endregion
}
