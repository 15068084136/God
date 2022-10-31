using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocal;
using UnityEngine.UI;
using System;

/// <summary>
/// 任务奖励页面
/// </summary>
public class TaskWin : WinRoot
{
    public Transform scrollTrans;

    PlayerData pd;
    private List<TaskData> taskDatas = new List<TaskData>();

    protected override void InitWin()
    {
        base.InitWin();
        pd = GameRoot.instance.PlayerData;
        RefreshUI();
    }

    public void RefreshUI(){
        taskDatas.Clear();

        List<TaskData> todoList = new List<TaskData>();
        List<TaskData> doneList = new List<TaskData>();

        // 1|0|0
        for (int i = 0; i < pd.taskArr.Length; i++)
        {
            string[] taskInfo = pd.taskArr[i].Split("|");
            TaskData taskData = new TaskData{
                ID = int.Parse(taskInfo[0]),
                prgs = int.Parse(taskInfo[1]),
                isToken = taskInfo[2].Equals("1")
            };
            if(taskData.isToken){
                doneList.Add(taskData);
            }else{
                todoList.Add(taskData);
            }
        }

        // 先放未完成，后方已完成
        taskDatas.AddRange(todoList);
        taskDatas.AddRange(doneList);

        // 在实例化前先销毁之前生成的物体
        for (int i = 0; i < scrollTrans.childCount; i++)
        {
            Destroy(scrollTrans.GetChild(i).gameObject);
        }

        // 实例化
        for (int i = 0; i < taskDatas.Count; i++)
        {
            GameObject go = resService.LoadPrefeb(Constants.TaskItemPrefeb, scrollTrans);
            go.name = "Task_" + i;

            TaskData taskData = taskDatas[i];
            TaskCfg taskCfg = resService.GetTaskRewardCfg(taskData.ID);
            // 设置任务名称
            SetText(GetTrans(go.transform, "TaskNameTxt"), taskCfg.taskName);
            // 设置任务进度
            SetText(GetTrans(go.transform, "TaskPrgTxt"), taskData.prgs + "/" + taskCfg.count);
            SetText(GetTrans(go.transform, "GainExp"), "奖励：     经验" + taskCfg.exp);
            SetText(GetTrans(go.transform, "GainCoin"), "金币" + taskCfg.coin);
            // 设置任务进度图片
            Image image = GetTrans(go.transform, "PrgImage").GetComponent<Image>();
            float taskPrgs = taskData.prgs * 1.0f/taskCfg.count;
            image.fillAmount = taskPrgs;
            SetText(GetTrans(go.transform, "TaskNameTxt"), taskCfg.taskName);
            // 设置按钮点击事件
            Button takeButton = GetTrans(go.transform, "TaskBtn").GetComponent<Button>();
            takeButton.onClick.AddListener(()=>{
                ClickTakeButton(go.name);
            });
            // 设置领取任务奖励字段
            Transform gainTrans = GetTrans(go.transform, "AlreadyGainImage");
            // 如果任务已经被领取
            if(taskData.isToken){
                takeButton.interactable = false;
                SetActive(gainTrans);
            }else{
                // 任务未被领取
                SetActive(gainTrans, false);
                if(taskData.prgs == taskCfg.count){
                    takeButton.interactable = true;
                }else{
                    takeButton.interactable = false;
                }
            }
        }
    }

    private void ClickTakeButton(string name)
    {
        string[] nameArr = name.Split("_");
        int index = int.Parse(nameArr[1]);
        GameMsg msg = new GameMsg{
            cmd = (int)Command.reqTakeRewardTask,
            reqTakeTaskReward = new ReqTakeTaskReward{
                taskRewardId = taskDatas[index].ID
            }
        };
        netService.SendMsg(msg);

        TaskCfg taskCfg = resService.GetTaskRewardCfg(taskDatas[index].ID);
        int coin = taskCfg.coin;
        int exp = taskCfg.exp;
        GameRoot.AddTips(Constants.ColorText("获得奖励:", TxtColor.blue) + Constants.ColorText(" 金币 + " + coin + " 经验 + " + exp, TxtColor.green));
    }

    public void ClickCloseBtn(){
        SetWinState(false);
    }
}
