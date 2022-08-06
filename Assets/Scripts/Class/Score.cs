using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Score 
{
    public string playerName;//玩家名字
    public int killNum;//击杀数
    public int waveNum;//击败波数
    public float saveTime;//生存时间
    
    public List<int> UScore = new List<int>();
    public List<int> UWave = new List<int>();
    public List<float> UTime = new List<float>();

}
