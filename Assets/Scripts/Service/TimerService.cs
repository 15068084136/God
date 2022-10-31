using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 计时器服务
/// </summary>
public class TimerService : SystemRoot
{
    public static TimerService Instance = null;
    PETimer pt;
    public void InitService(){
        Instance = this;
        pt = new PETimer();
        // 设置日志
        pt.SetLog((string info)=>{
            PECommon.Log(info);
        });
        PECommon.Log("Init TimerService...");
    }
    // 相应，延时时间，时间单位是毫秒，循环一次
    public int AddTimeTask(Action<int> callBack, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1){
       // 返回时间任务的ID号
       return pt.AddTimeTask(callBack, delay, timeUnit, count);
    }

    public void Update() {
        pt.Update();
    }

    public double GetNowTime(){
        return pt.GetMillisecondsTime();
    }

    // 删除时间任务
    public void DeleteTask(int tid){
        pt.DeleteTimeTask(tid);
    }
}
