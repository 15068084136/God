using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioServer : MonoBehaviour
{
    public static AudioServer instance;
    public AudioSource BGMAudio;
    public AudioSource UIAudio;

    public void InitService(){
        instance = this;
    }

    public void StopBGMAudio(){
        if(BGMAudio != null){
            BGMAudio.Stop();
        }
    }
    
    public void PlayBGMAudio(string name, bool loop = true){
        AudioClip clip = ResService.instance.LoadAudio("ResAudio/" + name, true);
        if(BGMAudio.clip == null || BGMAudio.clip.name != clip.name){
            BGMAudio.clip = clip;
            BGMAudio.loop = loop;
            BGMAudio.Play();
        }
    }

    public void PlayUIAudio(string name){
        AudioClip clip = ResService.instance.LoadAudio("ResAudio/" + name, true);
        UIAudio.clip = clip;
        UIAudio.Play();
    }
}
