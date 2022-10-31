using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PENet;
using PEProtocal;

public class ClientSession : PESession<GameMsg>
{
    protected override void OnConnected()
    {
        PECommon.Log("Service Connect");
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("Server Response:");
        NetService.instance.AddNetPkg(msg);
    }

    protected override void OnDisConnected()
    {
        PECommon.Log("Server Disconnect");
    }
}
