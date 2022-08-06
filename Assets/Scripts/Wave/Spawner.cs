using System;
using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;//获取敌人预制体

    #region Wave
    public Wave[] Waves;//自定义类的数组 - 波数
    private Wave currentWave;//当前波数
    private int currentIndex;//当前波数在数组集合种的【索引index】
    public int waitSpawnNum;//这一波还剩下多少敌人没有生成
    public int spawnAliveNum;//这一波的敌人还存活多少个
    public float nextSpawnTime;
    #endregion

    private MapGenerator mapGenerator;//获取地图生成脚本
    private bool isDisable;//敌人生成开关
    private LivingEntity playerEntity;//获取玩家脚本

    public event Action<int> onNewWave;//事件，在NextWave内部逻辑触发，因为每波序号不同，所以用Action<int>
    
    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        playerEntity = FindObjectOfType<PlayerController>();

        playerEntity.onDeath += PlayerDeath;

        NextWave();
    }
    void ResetPlayerPos()
    {
        playerEntity.transform.position = Vector3.zero + Vector3.up * 1;//玩家掉落
    }
    //生成敌人
    void Update()
    {
        if(!isDisable)//如果玩家已经死亡，不运行
        {
            if((waitSpawnNum > 0 || currentWave.infinite)&& Time.time>nextSpawnTime)//当前波敌人未召唤完 且 游戏运行时间大于下次召唤时间
            {
                waitSpawnNum--;//当前波敌人数量--
                nextSpawnTime = Time.time + currentWave.timeBtwSpawn;//下一次生成时间间隔timeBtwSpawn秒
                //GameObject spawnEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);//生成敌人
                StartCoroutine(SpawnEnemy());//敌人生成改成使用协程调用
            }
        }
    }
    void NextWave()
    {
        currentIndex++;
        Debug.Log(string.Format("Current Wave :{0}", currentIndex));

        if(currentIndex - 1 < Waves.Length)//最后一步，这次currentIndex是从1开始的，所以第三波的时候，index实际上等于3已经超过了Length的范围，所以再下一波的时候报错，我们可以限定范围
        {
            currentWave = Waves[currentIndex - 1];//一开始index = 1
            waitSpawnNum = currentWave.enemyNum;
            spawnAliveNum = currentWave.enemyNum;

            if(onNewWave != null)//如果事件为空
            {
                onNewWave(currentIndex);//FIXME
            }
        }
        ResetPlayerPos();
        FindObjectOfType<GameUI>().NewWaveBannerUI(currentIndex);//UI display
    }
    IEnumerator SpawnEnemy()//生成敌人
    {
        float spawnDelay = 1.0f;
        float tileFlashSpeed = 4;

        Transform randomTile = mapGenerator.GetRandomOpenTile();
        
        #region 颜色变换
        Material tileMaterial = randomTile.GetComponent<MeshRenderer>().material;
        Color originalColor = Color.white;//地板原始颜色--white
        Color flashColor = Color.red;//闪烁颜色--red
        float spawnTimer = 0;
        #endregion
        
        while(spawnTimer < spawnDelay)
        {
            tileMaterial.color = Color.Lerp(originalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        GameObject spawnEnemy = Instantiate(enemyPrefab, randomTile.position + Vector3.up, Quaternion.identity);
        spawnEnemy.GetComponent<EnemyController>().onDeath += EnemyDeath;
        //难度设置。当前波敌人速度，伤害，生命值，颜色
        spawnEnemy.GetComponent<EnemyController>().SetDifficulty(currentWave.enemySpeed, currentWave.enemyDamage, currentWave.enemyHealth, currentWave.enemySkinColor);
    }
    private void EnemyDeath()
    {
        spawnAliveNum--;
        if(spawnAliveNum <= 0) //敌人全部阵亡则下一波
        {
            NextWave();
        }
    }
    private void PlayerDeath()
    {
        isDisable = true;
    }
}
