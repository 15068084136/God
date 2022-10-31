using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表现实体怪物控制器
/// </summary>
public class MonsterController : Controller
{
    private void Update() {
        // AI逻辑表现
        if(isMove){
            SetDir();
            if(!battleManager.isPauseGame){
                SetMove();
            }
        }
    }
    private void SetMove()
    {
        characterController.Move(transform.forward * Time.deltaTime * Constants.MonsterMoveSpeed);
        // 解决资源问题所写的代码
        characterController.Move(Vector3.down * Time.deltaTime * Constants.MonsterMoveSpeed);
    }
    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
}
