using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 有限状态机，状态接口
/// </summary>
public interface IState{
    // 传入变长参数
    void Enter(EntityBase entity, params object[] args){

    }
    void Process(EntityBase entity, params object[] args){

    }
    void Exit(EntityBase entity, params object[] args){

    }
}

public enum AniState{
    none, 
    born,
    idle,
    move,
    attack,
    hit,
    die
}
