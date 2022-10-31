using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemRoot : MonoBehaviour
{
    protected ResService resService;
    protected AudioServer audioServer;
    protected NetService netService;
    protected TimerService timerService;

    public virtual void InitSys(){
        resService = ResService.instance;
        audioServer = AudioServer.instance;
        netService = NetService.instance;
        timerService = TimerService.Instance;
    }
}
