using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.IO;
using UnityEngine.Networking;

[Hotfix]
public class LuaService : MonoBehaviour
{
    public static LuaService instance;

    LuaEnv luaEnv;

    public void InitService(){
        instance = this;

        luaEnv = new LuaEnv();

        luaEnv.AddLoader(MyLoader);

        StartCoroutine(LoadResourceCorotine());
    }

    private byte[] MyLoader(ref string filepath)
    {
        string abstractPath = @"C:\Users\ASUS\Desktop\unity\PersonalGame\GodLike\Client\Lua\" + filepath + ".lua.txt";
        return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(abstractPath));
    }

    private void OnDisable() {
        
        luaEnv.DoString("require 'LuaDispose'");
    }

    private void OnDestroy() {
        luaEnv.Dispose();    
    }

    IEnumerator LoadResourceCorotine(){
        UnityWebRequest request = UnityWebRequest.Get(@"http://localhost:51911/LuaTest.lua.txt");
        yield return request.SendWebRequest();
        string str = request.downloadHandler.text;
        // 将这部分内容写进一个文本文件，并覆盖到相应地址
        File.WriteAllText(@"C:\Users\ASUS\Desktop\unity\PersonalGame\GodLike\Client\Lua\LuaTest.lua.txt", str); 
        
        luaEnv.DoString("require 'LuaTest'");
        
        UnityWebRequest request2 = UnityWebRequest.Get(@"http://localhost:51911/LuaDispose.lua.txt");
        yield return request2.SendWebRequest();
        string str2 = request2.downloadHandler.text;
        // 将这部分内容写进一个文本文件，并覆盖到相应地址
        File.WriteAllText(@"C:\Users\ASUS\Desktop\unity\PersonalGame\GodLike\Client\Lua\LuaDispose.lua.txt", str2); 
   }

    [LuaCallCSharp]
   public int GetListCount(List<string> stringList){
        return stringList.Count;
   }

   [LuaCallCSharp]
   public string GetListContent(List<string> stringList, int index){
    return stringList[index];
   }
}
