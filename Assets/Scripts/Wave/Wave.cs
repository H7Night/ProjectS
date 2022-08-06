using UnityEngine;

[System.Serializable]
public class Wave //Struct ScriptableObject都可以
{
    public bool infinite;
    public int enemyNum;//每波敌人总数
    public float timeBtwSpawn;//每波敌人出现的 间隔时间

    //【Difficulty】
    public float enemySpeed;
    public int enemyDamage;
    public float enemyHealth;
    public Color enemySkinColor;
}
