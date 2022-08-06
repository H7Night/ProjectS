using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MapGenerator : MonoBehaviour
{
    #region 生成瓦片
    public GameObject tilePrefab;
    //public Vector2 mapSize;
    //public Transform mapHolider;
    [Range(0,1)]public float outlinePresent;//瓦片间缝隙
    #endregion
    
    #region 障碍物
    List<Coord> allTilesCoords; //不用public List<Coord> TilesCoord = new List<Coord>();
    Queue<Coord> shuffleTileQueue;//洗牌队列
    public GameObject obsPrefab;//获取障碍物
    #endregion

    #region 地图缝隙
    [Range(0, 1)] public float obsPercent;
    #endregion

    #region navMesh
    public float tileSize;//用来调整整个地图大小
    public GameObject mapFloor;
    public GameObject navMeshFloor;
    public Vector2 mapMaxSize;
    public GameObject navMeshObs;//空气墙
    #endregion

    #region Map
    public Map[] maps;//用来存储地图的数组
    public int mapIndex;//地图数组的索引
    public Map currentMap;//当前地图，public 是为了给摄像机获取
    #endregion
    
    // #region Map
    // [Header("Map Fully Accessible")]
    // private Coord mapCenter;//任何随机地图，中心点不能有障碍物，这个点用于人物生成，和填充算法判定用
    // bool[,] mapObstacles;//判断任何坐标位置是否有障碍物
    // #endregion

    Transform[,] tilemap;
    Queue<Coord> shuffleOpenTileCoords;//线性表，先进先出
    //public GameObject player;
    void Awake()
    {
        FindObjectOfType<Spawner>().onNewWave += NewWave;
    }
    private void NewWave(int _waveIndex)
    {
        mapIndex = _waveIndex - 1;
        GenerateMap();
    }

    //生成地图
    void GenerateMap()
    {   
        currentMap = maps[mapIndex];
        tilemap = new Transform[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];//获取当前地图的mapsize大小

        System.Random prng = new System.Random(currentMap.seed);//random seed
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.05f, currentMap.mapSize.y * tileSize);//长宽为当前地图的长宽 * 整体大小

        allTilesCoords = new List<Coord>();//所有瓦片坐标;每次开启一个新的地图都会进行"初始化"。

        #region mapHolder
        string holderName = "mapHolder";
        if(transform.Find(holderName))
        {
            //如果找到mapHolder 删除，波数切换地图时使用，初始没有mapHolder
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        Transform mapHolder = new GameObject(holderName).transform;//new一个mapHolder空物体，获得它的transform
        mapHolder.parent = transform;//设置new出来的mapHolder的父物体，是挂在这个脚本的物体
        #endregion

        #region 地图瓦片生成
        for (int i = 0; i < currentMap.mapSize.x; i++)//横
        {
            for (int j = 0; j < currentMap.mapSize.y; j++)//竖
            {
                Vector3 newPos = CoordToPosition(i, j);
                //生成地板
                GameObject spawnTile = Instantiate(tilePrefab, newPos, Quaternion.Euler(90, 0, 0));
                spawnTile.transform.parent = mapHolder;//父物体
                spawnTile.transform.localScale *= (1 - outlinePresent) * tileSize;//缝隙

                allTilesCoords.Add(new Coord(i, j));
                tilemap[i, j] = spawnTile.transform;
            }
        }
        #endregion

        #region 障碍物生成
        //洗牌的瓦片队列
        shuffleTileQueue = new Queue<Coord>(Utilities.ShuffleArray(allTilesCoords.ToArray(), currentMap.seed));

        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
        
        int obsCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obsPersent);
        int currentObsCount = 0;//当前障碍物数量
        List<Coord> allOpenCoords = new List<Coord>(allTilesCoords);//一开始等于allTileCoords

        for (int i = 0; i < obsCount; i++)
        {
            Coord randomCoord = GetRandomCoord();//队列先进先出
            obstacleMap[randomCoord.x, randomCoord.y] = true;//假设当前位置可生成
            currentObsCount++;//每次生成障碍物，currentObsCount数量++，统计障碍物数量

            //调用MapIsFullyAccessible泛洪算法判断
            if(randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap,currentObsCount))
            {
                float obsHeight = Mathf.Lerp(currentMap.minObsHeight, currentMap.maxObsHeight, (float)prng.NextDouble());//Random.NextDouble() --> 返回一个大于或等于 0.0 且小于 1.0 的随机浮点数。
                
                Vector3 newPos = CoordToPosition(randomCoord.x, randomCoord.y);
                GameObject spawnObs = Instantiate(obsPrefab, newPos + Vector3.up * obsHeight / 2, Quaternion.identity);
                spawnObs.transform.parent = mapHolder;
                spawnObs.transform.localScale = new Vector3((1 - outlinePresent), obsHeight, (1 - outlinePresent)) * tileSize;

                MeshRenderer renderer = spawnObs.GetComponent<MeshRenderer>();
                Material material = renderer.material;
                float colorPercent = randomCoord.y / currentMap.mapSize.y;
                //颜色
                material.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                renderer.material = material;

                allOpenCoords.Remove(randomCoord);
            }
            else//如果方法返回值为false，说明false的地方不能生成障碍
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObsCount--;
            }
        }
        #endregion

        shuffleOpenTileCoords = new Queue<Coord>(Utilities.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));

        #region 空气墙
        //动态的创建“空气墙”NavMeshObstacle + Collider
        //Up(Forward)-Down(Back)-Left-Right
        navMeshFloor.transform.localScale = new Vector3(mapMaxSize.x, mapMaxSize.y) * tileSize;
        mapFloor.transform.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);

        GameObject navMeshObsUp = Instantiate(navMeshObs, Vector3.forward * (currentMap.mapSize.y + mapMaxSize.y) / 4f * tileSize, Quaternion.identity);
        navMeshObsUp.transform.parent = mapHolder;
        navMeshObsUp.transform.localScale = new Vector3(currentMap.mapSize.x, 5f, mapMaxSize.y / 2f - currentMap.mapSize.y / 2f);
        
        GameObject navMeshObsDown = Instantiate(navMeshObs, Vector3.back * (currentMap.mapSize.y + mapMaxSize.y) / 4f, Quaternion.identity);
        navMeshObsDown.transform.parent = mapHolder;
        navMeshObsDown.transform.localScale = new Vector3(currentMap.mapSize.x, 5f, mapMaxSize.y / 2 - currentMap.mapSize.y / 2f);

        GameObject navMeshObsLeft = Instantiate(navMeshObs, Vector3.left * (currentMap.mapSize.x + mapMaxSize.x) / 4f * tileSize, Quaternion.identity);
        navMeshObsLeft.transform.parent = mapHolder;
        navMeshObsLeft.transform.localScale = new Vector3((currentMap.mapSize.x / 2f - mapMaxSize.x / 2f), 1, currentMap.mapSize.y);

        GameObject navMeshObsRight = Instantiate(navMeshObs, Vector3.right * (currentMap.mapSize.x + mapMaxSize.x) / 4f * tileSize, Quaternion.identity);
        navMeshObsRight.transform.parent = mapHolder;
        navMeshObsRight.transform.localScale = new Vector3((currentMap.mapSize.x / 2f - mapMaxSize.x / 2f), 5, currentMap.mapSize.y);
        #endregion
    
    }

    //泛洪算法，洪水填充
    private bool MapIsFullyAccessible(bool[,] _mapObstacles, int _currentObsCount)//传进参数bool数组 ; 假设障碍物的数量
    {
        bool[,] mapFlags = new bool[_mapObstacles.GetLength(0), _mapObstacles.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();//所有的坐标都会“筛选后”存储在这个队列中，--检测
        queue.Enqueue(currentMap.mapCenter);//Enqueue -- 入队
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;//中心点已标记为【已检测】
       
        int accessibldCount = 1; //因为MapCenter无论何时，都是可以行走的
        while (queue.Count > 0)//队列只要大于0就会一直检测
        {
            Coord currentTile = queue.Dequeue();//检测到的移除; Dequeue -- 出队
            for (int i = -1; i <= 1; i++)//检测四周的坐标点X轴
            {
                for (int j = -1; j <= 1; j++)//检测四周的坐标点Y轴
                {
                    int neighborX = currentTile.x + i;
                    int neighborY = currentTile.y + j;

                    if (i == 0 || j == 0)//保证上下左右四个位置，排除对角方向的坐标位置
                    {
                        //防止相邻的点，超出地图范围
                        if (neighborX >= 0 && neighborX < _mapObstacles.GetLength(0) && neighborY >= 0 && neighborY < _mapObstacles.GetLength(1))
                        {
                            //保证相邻点：1、还没被检测。mapFlags为false。2、mapObstacle也为fasle没有障碍物
                            if (!mapFlags[neighborX, neighborY] && !_mapObstacles[neighborX, neighborY])
                            {
                                mapFlags[neighborX, neighborY] = true;//标记已经为检查过
                                accessibldCount++;//没有障碍物，可行走位置++
                                queue.Enqueue(new Coord(neighborX, neighborY));//将相邻位置添加到队列中，进入下一轮遍历
                            }
                        }
                    }
                }
            }
        }
        int obsTargtCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - _currentObsCount);//假设可行走数量 = 地图总瓦片数 - 假设障碍物数量 
        return accessibldCount == obsTargtCount;
        //如果可生成区域数量和假设生成数量相同，return true，说明地图可通行.如果为false表示这个位置生成后阻碍原地图连接完整性
    }
    public Coord GetRandomCoord()//随机坐标
    {
        Coord randomCoord = shuffleTileQueue.Dequeue();
        shuffleTileQueue.Enqueue(randomCoord);//将移除的匀速，放在队列最后一个，保证队列完整性，大小不变
        return randomCoord;//返回队列第一个元素
    }
    public Transform GetRandomOpenTile()//随机瓦片--用于召唤敌人
    {
        Coord randomCoord = shuffleOpenTileCoords.Dequeue();
        shuffleOpenTileCoords.Enqueue(randomCoord);
        return tilemap[randomCoord.x, randomCoord.y];
    }
    public Vector3 CoordToPosition(int _x,int _y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + _x, 0, -currentMap.mapSize.y / 2f + 0.5f + _y) * tileSize;
    }
}

//class,struct想要在inspector窗口中展示，需要序列化
[System.Serializable]
public struct Coord
{
    public int x, y;
    public Coord(int _x, int _y)
    {
        this.x = _x;
        this.y = _y;
    }
    public static bool operator !=(Coord _c1, Coord _c2)
    {
        return !(_c1 == _c2);
    }
    public static bool operator ==(Coord _c1, Coord _c2)
    {
        return (_c1.x == _c2.x) && (_c1.y == _c2.y);
    }
}

[System.Serializable]
public class Map
{
    public Vector2 mapSize;//大小
    [Range(0, 1)] public float obsPersent;//障碍物占百分比
    public int seed;//地图种子
    public float minObsHeight, maxObsHeight;//障碍物最低最高高度
    public Color foregroundColor, backgroundColor;//前后景颜色
    public Coord mapCenter//地图中心
    {
        get
        {
            return new Coord((int)(mapSize.x / 2), (int)(mapSize.y / 2));
        }
    }
}


