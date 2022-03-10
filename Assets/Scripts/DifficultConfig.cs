using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultConfig 
{
    public int width;
    public int height;
    public int mineCount;
   public DifficultConfig(int width, int height, int mineCount)
    {
        this.width = width;
        this.height = height;
        this.mineCount = mineCount;
    }
}
public class DifficultConfigConstance
{
    public static readonly DifficultConfig[] p = { new DifficultConfig(9, 9, 10), new DifficultConfig(16, 16, 40) , new DifficultConfig(20, 20, 60) };
    
}

