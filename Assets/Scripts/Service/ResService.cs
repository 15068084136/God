using System.Threading.Tasks;
using System.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 数据加载服务
/// </summary>
public class ResService : MonoBehaviour
{
    public static ResService instance;

    public void InitService(){
        instance = this;
        InitRandomNameCfg(Constants.RandomNameCfgPath);
        InitMonsterCfg(Constants.monsterCfgPath);
        InitMapCfg(Constants.MapCfgPath);
        InitGuideCfg(Constants.GuideCfgPath);
        InitStrongCfg(Constants.strongPath);
        InitTaskRewardCfg(Constants.taskRewardCfgPath);
        InitSkillCfg(Constants.skillCfgPath);
        InitSkillMoveCfg(Constants.skillMoveCfgPath);
        InitSkillActionCfg(Constants.skillActionCfgPath);
        PECommon.Log("ResService Init Down");
    }

    // 委托，帮助异步加载场景内部逻辑多次实现，避免只在开始调用一次
    private Action progressCB = null;
    /// <summary>
    /// 异步加载场景
    /// </summary>
    public void AsyncLoadScene(string sceneName, Action loaded){
        // 加载界面显示
        GameRoot.instance.loadingWin.SetWinState();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        // 在progressCB不为空的时候，重复调用后面的函数
        progressCB = () =>{
        float value = operation.progress;
        GameRoot.instance.loadingWin.SetProgress(value);
        // 加载完成
        if(value == 1){
            // 打开登录界面
            //LoginSys.instance.OpenLoginWin();
            // 如果loaded不为空，则调用回调函数
            if(loaded != null){
                loaded();
            }
            progressCB = null;
            operation = null;
            GameRoot.instance.loadingWin.SetWinState(false);
        }
        };
    }

    private void Update() {
        if(progressCB != null){
            progressCB();
        }
    }

    // 加载音乐
    private Dictionary<string, AudioClip> audioDic = new Dictionary<string, AudioClip>();
    public AudioClip LoadAudio(string path, bool cache){
        AudioClip clip;
        if(!audioDic.TryGetValue(path, out clip)){
            clip = Resources.Load<AudioClip>(path);
            if(cache){
                audioDic.Add(path, clip);
            }
        }
        return clip;
    }

    // 加载预制体
    private Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
    public GameObject LoadPrefeb(string path, bool cache = false){
        GameObject prefeb = null;
        // 如果在缓存里没有找到预制体
        if(!goDic.TryGetValue(path, out prefeb)){
            prefeb = Resources.Load<GameObject>(path);
            if(cache){
                goDic.Add(path, prefeb);
            }
        }
        GameObject go = null;
        if(prefeb != null){
            go = Instantiate(prefeb);
        }
        return go;
    }
        public GameObject LoadPrefeb(string path, Transform parent,bool cache = false){
        GameObject prefeb = null;
        // 如果在缓存里没有找到预制体
        if(!goDic.TryGetValue(path, out prefeb)){
            prefeb = Resources.Load<GameObject>(path);
            if(cache){
                goDic.Add(path, prefeb);
            }
        }
        GameObject go = null;
        if(prefeb != null){
            go = Instantiate(prefeb, parent);
        }
        return go;
    }

    // 加载图片
    private Dictionary<string, Sprite> spDic = new Dictionary<string, Sprite>();
    public Sprite LoadSprite(string path, bool cache = false){
        Sprite sp = null;
        if(!spDic.TryGetValue(path, out sp)){
            sp = Resources.Load<Sprite>(path);
            if(cache){
                spDic.Add(path, sp);
            }
        }
        return sp;
    }

    #region 配置文件
    private List<string> surnameList = new List<string>();
    private List<string> manList = new List<string>();
    private List<string> womanList = new List<string>();

    // 加载名字配置文件
    private void InitRandomNameCfg(string path){
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(!xml){
            PECommon.Log("未找到文件");
        }
        else{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            // 获取根节点下的所有子节点
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;

            // 遍历所有子节点
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement element = xmlNodeList[i] as XmlElement;

                // 如果ID节点的属性为空，则不继续下面的代码，直接进入下一个节点
                if(element.GetAttributeNode("ID") == null){
                    continue;
                } 

                int ID = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);

                // 遍历ID节点下所有的子节点
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "surname":
                            surnameList.Add(e.InnerText);
                            break;
                        case "man":
                            manList.Add(e.InnerText);
                            break;
                        case "woman":
                            womanList.Add(e.InnerText);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    // 获得名字
    public string GetRandomName(bool man = true){
        System.Random rd = new System.Random();
        string randomName = surnameList[PETools.RandomInt(0, surnameList.Count - 1)];
        // 如果是男性，就加上男性的名
        if(man){
            randomName += manList[PETools.RandomInt(0, manList.Count - 1)];
        }else{
            randomName += womanList[PETools.RandomInt(0, womanList.Count - 1)];
        }

        return randomName;
    }

    #region 地图
    // 加载地图配置文件
    private Dictionary<int, MapCfg> mapCfgDataDic = new Dictionary<int, MapCfg>();
    public void InitMapCfg(string path){
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(!xml){
            PECommon.Log("未找到文件");
        }
        else{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            // 获取根节点下的所有子节点
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;

            // 遍历所有子节点
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement element = xmlNodeList[i] as XmlElement;

                // 如果ID节点的属性为空，则不继续下面的代码，直接进入下一个节点
                if(element.GetAttributeNode("ID") == null){
                    continue;
                } 

                int ID = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                MapCfg mc = new MapCfg{
                    ID = ID,
                    monsterList = new List<MonsterData>()
                };

                // 遍历ID节点下所有的子节点
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "mapName":
                            mc.mapName = e.InnerText;
                            break;
                        case "sceneName":
                            mc.sceneName = e.InnerText;
                            break;
                        case "power":
                            mc.power = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            mc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            mc.exp = int.Parse(e.InnerText);
                            break;
                        case "crystal":
                            mc.crystal = int.Parse(e.InnerText);
                            break;
                        case "mainCamPos":{
                            // 通过逗号分隔得到字符串数组
                            string[] valArr = e.InnerText.Split(",");
                            mc.mainCamPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),float.Parse(valArr[2]));
                        }break;
                        case "mainCamRote":{
                            // 通过逗号分隔得到字符串数组
                            string[] valArr = e.InnerText.Split(",");
                            mc.mainCamRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),float.Parse(valArr[2]));
                        }
                            break;
                        case "playerBornPos":{
                            // 通过逗号分隔得到字符串数组
                            string[] valArr = e.InnerText.Split(",");
                            mc.playerBornPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),float.Parse(valArr[2]));
                        }
                            break;
                        case "playerBornRote":{
                            // 通过逗号分隔得到字符串数组
                            string[] valArr = e.InnerText.Split(",");
                            mc.playerBornRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),float.Parse(valArr[2]));
                        }
                            break;
                        case "monsterLst":
                            string[] valueArr = e.InnerText.Split("#");
                            for (int waveIndex = 0; waveIndex < valueArr.Length; waveIndex++)
                            {
                                // ＃前面第一个数据为空
                                if(waveIndex == 0){
                                    continue;
                                }
                                string[] tempArr = valueArr[waveIndex].Split("|");
                                for (int j = 0; j < tempArr.Length; j++)
                                {
                                    if(j == 0){
                                        continue;
                                    }
                                    string[] arr = tempArr[j].Split(",");
                                    MonsterData md = new MonsterData{
                                        ID = int.Parse(arr[0]),
                                        mWave = waveIndex,
                                        mIndex = j,
                                        monsterCfg = GetMonsterCfg(int.Parse(arr[0])),
                                        mBornPos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3])),
                                        mBornRote = new Vector3(0, float.Parse(arr[4]), 0),
                                        mLevel = int.Parse(arr[5])
                                    };
                                    mc.monsterList.Add(md);
                                }
                            }
                            break;
                    }
                }
                mapCfgDataDic.Add(ID, mc);
            }
        }
    }
    public MapCfg GetMapCfg(int id){
        MapCfg data;
        if(mapCfgDataDic.TryGetValue(id, out data)){
            return data;
        }
        return null;
    }
    #endregion
    
    #region 任务引导Cfg
    // 加载任务引导系统配置
    private Dictionary<int, AutoGuideCfg> guildTaskDic = new Dictionary<int, AutoGuideCfg>();
    public void InitGuideCfg(string path){
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(!xml){
            PECommon.Log("未找到文件");
        }
        else{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            // 获取根节点下的所有子节点
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;

            // 遍历所有子节点
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement element = xmlNodeList[i] as XmlElement;

                // 如果ID节点的属性为空，则不继续下面的代码，直接进入下一个节点
                if(element.GetAttributeNode("ID") == null){
                    continue;
                } 

                int ID = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                AutoGuideCfg mc = new AutoGuideCfg{
                    ID = ID
                };

                // 遍历ID节点下所有的子节点
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "npcID":
                            mc.npcID = int.Parse(e.InnerText);
                            break;
                        case "dilogArr":
                            mc.dialogArr = e.InnerText;
                            break;
                        case "actID":
                            mc.actID = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            mc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            mc.exp = int.Parse(e.InnerText);
                            break;
                    }
                }
                guildTaskDic.Add(ID, mc);
            }
        }
    }
    public AutoGuideCfg GetGuildCfg(int id){
        AutoGuideCfg agc = null;
        if(guildTaskDic.TryGetValue(id, out agc)){
            return agc;
        }
        return null;
    }
    #endregion

    #region SkillCfg
    // 加载技能配置
    private Dictionary<int, SkillCfg> skillDic = new Dictionary<int, SkillCfg>();
    public void InitSkillCfg(string path){
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(!xml){
            PECommon.Log("未找到文件");
        }
        else{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            // 获取根节点下的所有子节点
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;

            // 遍历所有子节点
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement element = xmlNodeList[i] as XmlElement;

                // 如果ID节点的属性为空，则不继续下面的代码，直接进入下一个节点
                if(element.GetAttributeNode("ID") == null){
                    continue;
                } 

                int ID = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                SkillCfg mc = new SkillCfg{
                    ID = ID,
                    skillMoveLst = new List<int>(),
                    skillActionLst = new List<int>(),
                    skillDamageLst = new List<int>()
                };

                // 遍历ID节点下所有的子节点
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "skillName":
                            mc.skillName = e.InnerText;
                            break;
                        case "skillTime":
                            mc.skillTime = int.Parse(e.InnerText);
                            break;
                        case "aniAction":
                            mc.aniAction = int.Parse(e.InnerText);
                            break;
                        case "fx":
                            mc.fxName = e.InnerText;
                            break;
                        case "isCombo":
                            mc.isCombo = e.InnerText.Equals("1");
                            break;
                        case "isCollider":
                            mc.isCollider = e.InnerText.Equals("1");
                            break;
                        case "isBreak":
                            mc.isBreak = e.InnerText.Equals("1");
                            break;
                        case "cdTime":
                            mc.cdTime = int.Parse(e.InnerText);
                            break;
                        case "dmgType":
                            if(e.InnerText.Equals("1")){
                                mc.dmgType = DamageType.ad;
                            }else if(e.InnerText.Equals("2")){
                                mc.dmgType = DamageType.ap;
                            }else{
                                PECommon.Log("damgeType Error");
                            }
                            break;
                        case "skillMoveLst":
                            string[] skillMoveArr = e.InnerText.Split("|");
                            for (int j = 0; j < skillMoveArr.Length; j++)
                            {
                                if(skillMoveArr[j] != ""){
                                    mc.skillMoveLst.Add(int.Parse(skillMoveArr[j]));
                                }
                            }
                            break;
                        case "skillActionLst":
                            string[] skillActionArr = e.InnerText.Split("|");
                            for (int j = 0; j < skillActionArr.Length; j++)
                            {
                                if(skillActionArr[j] != ""){
                                    mc.skillActionLst.Add(int.Parse(skillActionArr[j]));
                                }
                            }
                            break;
                        case "skillDamageLst":
                            string[] skillDamageArr = e.InnerText.Split("|");
                            for (int j = 0; j < skillDamageArr.Length; j++)
                            {
                                if(skillDamageArr[j] != ""){
                                    mc.skillDamageLst.Add(int.Parse(skillDamageArr[j]));
                                }
                            }
                            break;
                    }
                }
                skillDic.Add(ID, mc);
            }
        }
    }
    public SkillCfg GetSkillCfg(int id){
        SkillCfg skillCfg = null;
        if(skillDic.TryGetValue(id, out skillCfg)){
            return skillCfg;
        }
        return null;
    }
    #endregion

    #region SkillActionCfg
    // 加载技能配置
    private Dictionary<int, SkillActionCfg> skillActionDic = new Dictionary<int, SkillActionCfg>();
    public void InitSkillActionCfg(string path){
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(!xml){
            PECommon.Log("未找到文件");
        }
        else{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            // 获取根节点下的所有子节点
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;

            // 遍历所有子节点
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement element = xmlNodeList[i] as XmlElement;

                // 如果ID节点的属性为空，则不继续下面的代码，直接进入下一个节点
                if(element.GetAttributeNode("ID") == null){
                    continue;
                } 

                int ID = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                SkillActionCfg mc = new SkillActionCfg{
                    ID = ID,
                };

                // 遍历ID节点下所有的子节点
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "delayTime":
                            mc.delayTime = int.Parse(e.InnerText);
                            break;
                        case "radius":
                            mc.radius = float.Parse(e.InnerText);
                            break;
                        case "angle":
                            mc.angle = int.Parse(e.InnerText);
                            break;
                    }
                }
                skillActionDic.Add(ID, mc);
            }
        }
    }
    public SkillActionCfg GetSkillActionCfg(int id){
        SkillActionCfg skillActionCfg = null;
        if(skillActionDic.TryGetValue(id, out skillActionCfg)){
            return skillActionCfg;
        }
        return null;
    }
    #endregion

    #region SkillMoveCfg
    // 加载技能配置
    private Dictionary<int, SkillMoveCfg> skillMoveDic = new Dictionary<int, SkillMoveCfg>();
    public void InitSkillMoveCfg(string path){
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(!xml){
            PECommon.Log("未找到文件");
        }
        else{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            // 获取根节点下的所有子节点
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;

            // 遍历所有子节点
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement element = xmlNodeList[i] as XmlElement;

                // 如果ID节点的属性为空，则不继续下面的代码，直接进入下一个节点
                if(element.GetAttributeNode("ID") == null){
                    continue;
                } 

                int ID = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                SkillMoveCfg mc = new SkillMoveCfg{
                    ID = ID
                };

                // 遍历ID节点下所有的子节点
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "moveTime":
                            mc.moveTime = int.Parse(e.InnerText);
                            break;
                        case "moveDis":
                            mc.moveDis = float.Parse(e.InnerText);
                            break;
                        case "delayTime":
                            mc.delayTime = int.Parse(e.InnerText);
                            break;
                    }
                }
                skillMoveDic.Add(ID, mc);
            }
        }
    }
    public SkillMoveCfg GetSkillMoveCfg(int id){
        SkillMoveCfg skillMoveCfg = null;
        if(skillMoveDic.TryGetValue(id, out skillMoveCfg)){
            return skillMoveCfg;
        }
        return null;
    }
    #endregion

    #region MonsterCfg
    // 加载怪物配置文件
    private Dictionary<int, MonsterCfg> monsterCfgDic = new Dictionary<int, MonsterCfg>();
    public void InitMonsterCfg(string path){
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(!xml){
            PECommon.Log("未找到文件");
        }
        else{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            // 获取根节点下的所有子节点
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;

            // 遍历所有子节点
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement element = xmlNodeList[i] as XmlElement;

                // 如果ID节点的属性为空，则不继续下面的代码，直接进入下一个节点
                if(element.GetAttributeNode("ID") == null){
                    continue;
                } 

                int ID = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                MonsterCfg mc = new MonsterCfg{
                    ID = ID,
                    battleProps = new BattleProps()
                };

                // 遍历ID节点下所有的子节点
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "mName":
                            mc.monsterName = e.InnerText;
                            break;
                        case "resPath":
                            mc.resPath = e.InnerText;
                            break;
                        case "mType":
                            if(e.InnerText.Equals("1")){
                                mc.mType = MonsterType.normal;
                            }else if(e.InnerText.Equals("2")){
                                mc.mType = MonsterType.boss;
                            }
                            break;
                        case "isStop":
                            mc.isStop = int.Parse(e.InnerText) == 1;
                            break;
                        case "hp":
                            mc.battleProps.hp = int.Parse(e.InnerText);
                            break;
                        case "ad":
                            mc.battleProps.ad = int.Parse(e.InnerText);
                            break;
                        case "ap":
                            mc.battleProps.ap = int.Parse(e.InnerText);
                            break;
                        case "addef":
                            mc.battleProps.addef = int.Parse(e.InnerText);
                            break;
                        case "apdef":
                            mc.battleProps.apdef = int.Parse(e.InnerText);
                            break;
                        case "dodge":
                            mc.battleProps.dodge = int.Parse(e.InnerText);
                            break;
                        case "pierce":
                            mc.battleProps.pierce = int.Parse(e.InnerText);
                            break;
                        case "critical":
                            mc.battleProps.critical = int.Parse(e.InnerText);
                            break;
                        case "skillID":
                            mc.skillID = int.Parse(e.InnerText);
                            break;
                        case "atkDis":
                            mc.atkDis = float.Parse(e.InnerText);
                            break;
                    }
                }
                monsterCfgDic.Add(ID, mc);
            }
        }
    }
    public MonsterCfg GetMonsterCfg(int id){
        MonsterCfg data;
        if(monsterCfgDic.TryGetValue(id, out data)){
            return data;
        }
        return null;
    }
    #endregion

    #region 强化Cfg
    // 嵌套字典
    // 根据强化图标位置信息，得到一个嵌套的字典，再根据星级得到对应的配置数据
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
    public void InitStrongCfg(string path){
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(!xml){
            PECommon.Log("未找到文件");
        }
        else{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            // 获取根节点下的所有子节点
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;

            // 遍历所有子节点
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement element = xmlNodeList[i] as XmlElement;

                // 如果ID节点的属性为空，则不继续下面的代码，直接进入下一个节点
                if(element.GetAttributeNode("ID") == null){
                    continue;
                } 

                int ID = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                StrongCfg sd = new StrongCfg{
                    ID = ID
                };

                // 遍历ID节点下所有的子节点
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    int val = int.Parse(e.InnerText);
                    switch (e.Name)
                    {
                        case "pos":
                            sd.pos = val;
                            break;
                        case "starlv":
                            sd.startLv = val;
                            break;
                        case "addhp":
                            sd.addHp = val;
                            break;
                        case "addhurt":
                            sd.addHurt = val;
                            break;
                        case "adddef":
                            sd.addDef = val;
                            break;
                        case "minlv":
                            sd.minLv = val;
                            break;
                        case "coin":
                            sd.coin = val;
                            break;
                        case "crystal":
                            sd.crystal = val;
                            break;
                    }
                }
                
                // 实际上是根据每一个ID节点创建了一个Dic
                Dictionary<int, StrongCfg> dic = null;
                if(strongDic.TryGetValue(sd.pos, out dic)){
                    dic.Add(sd.startLv, sd);
                }else{
                    dic = new Dictionary<int, StrongCfg>();
                    dic.Add(sd.startLv, sd);

                    strongDic.Add(sd.pos, dic);
                }
            }
        }
    }
    // 获得到某一个位置某一个星级的强化数据
    public StrongCfg GetStrongData(int pos, int startLv){
        StrongCfg sd = null;
        Dictionary<int, StrongCfg> dic = null;
        if(strongDic.TryGetValue(pos, out dic)){
            if(dic.ContainsKey(startLv)){
                sd = dic[startLv];
            }
        }
        return sd;
    }
    // 获得某一个位置某一个星级某一个属性的累加强化数据
    public int GetStrongVal(int pos, int startLv, int type){
        // 读取内部被嵌套的Dic
        Dictionary<int,StrongCfg> posDic = null;
        int val = 0;
        if(strongDic.TryGetValue(pos, out posDic)){
            for (int i = 0; i < startLv; i++)
            {
                StrongCfg sd;
                if(posDic.TryGetValue(i, out sd)){
                    switch (type)
                    {
                        case 1://hp
                            val += sd.addHp;
                            break;
                        case 2://hurt
                            val += sd.addHurt;
                            break;
                        case 3://def
                            val += sd.addDef;
                            break;
                    }
                }
            }
        }
        return val;
    }
    #endregion

    #region 加载TaskCfg
    private Dictionary<int, TaskCfg> taskRewardDic = new Dictionary<int, TaskCfg>();
    public void InitTaskRewardCfg(string path){
        TextAsset xml = Resources.Load<TextAsset>(path);
        if(!xml){
            PECommon.Log("未找到文件");
        }
        else{
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            // 获取根节点下的所有子节点
            XmlNodeList xmlNodeList = doc.SelectSingleNode("root").ChildNodes;

            // 遍历所有子节点
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlElement element = xmlNodeList[i] as XmlElement;

                // 如果ID节点的属性为空，则不继续下面的代码，直接进入下一个节点
                if(element.GetAttributeNode("ID") == null){
                    continue;
                } 

                int ID = Convert.ToInt32(element.GetAttributeNode("ID").InnerText);
                TaskCfg mc = new TaskCfg{
                    ID = ID
                };

                // 遍历ID节点下所有的子节点
                foreach (XmlElement e in xmlNodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "taskName":
                            mc.taskName = e.InnerText;
                            break;
                        case "count":
                            mc.count = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            mc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            mc.exp = int.Parse(e.InnerText);
                            break;
                    }
                }
                taskRewardDic.Add(ID, mc);
            }
        }
    }
    public TaskCfg GetTaskRewardCfg(int id){
        TaskCfg taskCfg = null;
        if(taskRewardDic.TryGetValue(id, out taskCfg)){
            return taskCfg;
        }
        return null;
    }
    #endregion
    #endregion
}
