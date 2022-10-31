using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocal;

/// <summary>
/// 战场管理器
/// </summary>
public class BattleManager : MonoBehaviour
{
    private ResService resService;
    private AudioServer audioServer;
    private StateManager stateMgr;
    private SkillManager skillMgr;
    private MapManager mapMgr;
    public PlayerEntity playerEntity;
    MapCfg mapCfg;
    private Dictionary<string, MonsterEntity> monsterDic = new Dictionary<string, MonsterEntity>();
    public void Init(int mapId, Action cb){
        resService = ResService.instance;
        audioServer = AudioServer.instance;
        // 初始化各大管理器
        stateMgr = gameObject.AddComponent<StateManager>();
        stateMgr.Init();
        skillMgr = gameObject.AddComponent<SkillManager>();
        skillMgr.Init();
        
        // 加载战场地图
        mapCfg = resService.GetMapCfg(mapId);
        resService.AsyncLoadScene(mapCfg.sceneName, ()=>{
            // 初始化地图数据
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            mapMgr = map.GetComponent<MapManager>();
            mapMgr.Init(this);

            map.transform.localPosition = Vector3.zero;
            map.transform.localScale = Vector3.one;

            // 设置摄像机的位置
            Camera.main.transform.position = mapCfg.mainCamPos;
            Camera.main.transform.eulerAngles = mapCfg.mainCamRote;

            LoadPlayer(mapCfg);
            // 默认是Idle状态
            playerEntity.Idle();

            // 激活第一批次的怪物
            ActiveMonster();

            audioServer.PlayBGMAudio(Constants.HuangYeBGM);

            if(cb != null){
                cb();
            }
        });
    }

    public bool triggerCheck = true;
    public bool isPauseGame = false;
    private void Update() {
        // 由于怪物逻辑实体实际上是不挂在物体上的，所以由battleMgr来驱动
        foreach (var item in monsterDic)
        {
            MonsterEntity em = item.Value;
            em.TickAiLogic();
        }

        // 检测当前批次的怪物是否已经全部死亡
        if(mapMgr != null){
            if(triggerCheck && monsterDic.Count == 0){
                bool isExist = mapMgr.SetDoorIsTrigger();
                triggerCheck = false;
                if(!isExist){
                    // 关卡已经打完
                    EndBattle(true, playerEntity.HP);
                }
            }
        }
    }

    public void EndBattle(bool isWin, int restHP){
        isPauseGame = false;
        AudioServer.instance.StopBGMAudio();
        BattleSys.instance.EndBattle(isWin, restHP);
    }

    public void LoadPlayer(MapCfg mapData){
        GameObject player = resService.LoadPrefeb(Constants.BattleplayerPrefebPath);
        player.transform.position = mapData.playerBornPos;
        player.transform.eulerAngles = mapData.playerBornRote;
        player.transform.localScale = Vector3.one;

        PlayerData pd = GameRoot.instance.PlayerData;
        BattleProps props = new BattleProps{
            hp = pd.hp,
            ad = pd.ad,
            ap = pd.ap,
            addef = pd.addef,
            apdef = pd.apdef,
            dodge = pd.dodge,
            pierce = pd.pierce,
            critical = pd.critical
        };

        // 实例化玩家逻辑实体
        playerEntity = new PlayerEntity();
        playerEntity.stateManager = stateMgr;
        playerEntity.skillManager = skillMgr;
        playerEntity.battleManager = this;
        playerEntity.Name = "AssassinBattle";

        // 在逻辑实体中设置玩家战斗数据
        playerEntity.SetBattleProps(props);

        // 表现实体和逻辑实体建立联系
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.Init();
        playerController.battleManager = this;
        playerEntity.SetCtrl(playerController);
    }

    // 根据怪物批次生成怪物
    public void LoadMonsterByWaveID(int wave){
        for (int i = 0; i < mapCfg.monsterList.Count; i++)
        {
            MonsterData md = mapCfg.monsterList[i];// 获得每一个怪物的配置文件
            if(md.mWave == wave){
                GameObject m = resService.LoadPrefeb(md.monsterCfg.resPath, true);
                m.transform.localPosition = md.mBornPos;
                m.transform.localEulerAngles = md.mBornRote;
                m.transform.localScale = Vector3.one;

                m.name = "m" + md.mWave + "-" + md.mIndex;

                MonsterEntity em = new MonsterEntity{
                    battleManager = this,
                    stateManager = stateMgr,
                    skillManager = skillMgr
                };
                // 将怪物数据注入到怪物逻辑实体内
                em.md = md;
                em.SetBattleProps(md.monsterCfg.battleProps);
                em.Name = m.name;

                MonsterController mc = m.GetComponent<MonsterController>();
                mc.Init();
                mc.battleManager = this;
                em.SetCtrl(mc);

                m.SetActive(false);
                monsterDic.Add(m.name, em);
                // 根据怪物类型生成不同的血条类型
                if(md.monsterCfg.mType == MonsterType.normal){
                    // 增加血条信息
                    GameRoot.instance.dynamicWin.AddHPItemInfo(m.name, mc.hpRootTrans.transform, em.HP);
                }else{
                    // 打开Boss血条
                    BattleSys.instance.playerContrlWin.SetBossHPBarState(true);
                }
                
            }
        }
    }
    
    public List<MonsterEntity> GetMonsterEntity(){
        List<MonsterEntity> monsterList = new List<MonsterEntity>();
        foreach (var item in monsterDic)
        {
            monsterList.Add(item.Value);
        }
        return monsterList;
    } 

    public void ActiveMonster(){
        TimerService.Instance.AddTimeTask((int tid)=>{
            foreach (var item in monsterDic)
            {
                item.Value.SetActive();
                item.Value.Born();
                TimerService.Instance.AddTimeTask((int id)=>{
                    // 出生一秒后进入Idle状态
                    item.Value.Idle();
                }, 1000);
            }
        }, 500);
    }

    //  从monsterDic中移除死亡的怪物
    public void RemoveMonster(string key){
        MonsterEntity monsterEntity;
        if(monsterDic.TryGetValue(key, out monsterEntity)){
            monsterDic.Remove(key);
            // 消除血条
            GameRoot.instance.dynamicWin.RemoveHPItemInfo(key);
        }
    }

    #region 技能施放与角色控制
    // 转接技能释放
    public void ReqReleaseSkill(int index){
        switch (index)
        {
            case 0:
                normalAttack();
                break;
            case 1:
                skill_1Attack();
                break;
            case 2:
                skill_2Attack();
                break;
            case 3:
                skill_3Attack();
                break;
        }
    }
    public void SetPlayerMoveDir(Vector2 dir){
        if(playerEntity.canControl == false){
            return;
        }

        // 设置玩家的移动
        if(dir == Vector2.zero){
            playerEntity.Idle();
            playerEntity.SetDir(Vector2.zero);
        }else{
            playerEntity.Move();
            playerEntity.SetDir(dir);
        }
    }
    
    public double laskAtkTime = 0;
    private int[] comboArr = new int[]{ 111, 112, 113, 114, 115 };
    public int comboIndex = 0;
    public void normalAttack(){
        if(playerEntity.currentState == AniState.attack){
            // 500ms内进行第二次点击，存数据
            double nowAtkTime = TimerService.Instance.GetNowTime();
            if(nowAtkTime - laskAtkTime < Constants.comboSpace && laskAtkTime != 0){
                if(comboArr[comboIndex] != comboArr[comboArr.Length - 1]){
                    comboIndex += 1;
                    playerEntity.comboQue.Enqueue(comboArr[comboIndex]);
                    laskAtkTime = nowAtkTime;
                }else{
                    laskAtkTime = 0;
                    comboIndex = 0;
                }
            }
        }else if(playerEntity.currentState == AniState.idle || playerEntity.currentState == AniState.move){
            comboIndex = 0;
            laskAtkTime = TimerService.Instance.GetNowTime();
            playerEntity.Attack(comboArr[comboIndex]);
        }
        playerEntity.Attack(111);
    }
    public void skill_1Attack(){
        playerEntity.Attack(101);
    }
    public void skill_2Attack(){
        playerEntity.Attack(102);
    }
    public void skill_3Attack(){
        playerEntity.Attack(103);
    }
    public Vector2 GetCurrnetDir(){
        return BattleSys.instance.GetCurrnetDir();
    }
    public bool CanRlsSkill(){
        return playerEntity.canControl;
    }
    #endregion
}
