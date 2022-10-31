using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PETools
{
    public static int RandomInt(int min, int max, System.Random rd = null){
        if(rd == null){
            rd = new System.Random();
        }
        int value = rd.Next(min, max + 1);
        return value;
    }
}
