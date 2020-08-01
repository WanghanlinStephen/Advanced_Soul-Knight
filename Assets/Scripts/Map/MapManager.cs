using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{

    [Header("地图种子")] public int seed;
    [Header("每行房间数")] public int mapMaxW;
    [Header("每列房间数")] public int mapMaxH;
    [Header("每间房装饰品的最大数量")] public int roomDecoratorMax;
    [Header("每间房装饰品的最小数量")] public int roomDecoratorMin;


    [Header("总生成房间数")] public int mapCount;
    [Header("最大房间宽")] public int roomMaxW;
    [Header("最大房间高")] public int roomMaxH;
    [Header("最小房间宽")] public int roomMinW;
    [Header("最小房间高")] public int roomMinH;
    [Header("最少特殊房间数")] public int specialroomMin;
    [Header("最多特殊房间数")] public int specialroomMax;
    [Header("房间的间隔距离")] public int distance;
    [Header("地板")] public TileBase floor;
    [Header("上下墙")] public TileBase wall;
    [Header("左右墙")] public TileBase wallLR;
    [Header("门")] public TileBase door;
    [Header("开门")] public TileBase OpenDoor;
    [Header("实心门")] public TileBase Realdoor;
    [Header("地Map")] public Tilemap tilemap;
    [Header("墙Map")] public Tilemap tilemapWall;
    [Header("门Map")] public Tilemap tilemapDoor;


    public GameObject[] WallArray;
    [Header("装饰1")] public TileBase block1;
    [Header("装饰2")] public TileBase block2;
    [Header("装饰3")] public TileBase block3;

    [Header("许愿池")] public TileBase pool;

    private List<MapSize> mapSize;
    private List<string> mapSpecialRoom;
    private List<SpecialRoomItem> SpecialRoomInfor;
    private List<int> SpecialRoomNumber;
    private List<Vector3Int> _centerPoint;
    private Dictionary<Vector3Int, int> _mapPoint;
    private List<Vector3Int> mapPoint;
    private List<Vector3Int> mapCenterPoint;
    public GameObject EnemyArea;
    public GameObject Wall;
    public GameObject Portal;
    public GameObject WishingPool;
    public GameObject Trader;
    public GameObject Table;
    public GameObject Treasure;
    private GameObject enteranceWallList;
    private GameObject RoomList;
    public GameObject DecoratorList;
    public GameObject PoolList;
    public GameObject StoreList;
    public GameObject TableList;
    public GameObject TreasureList;
    public static bool Enter = false;
    public GameObject TriggerObject;


    private void Start()
    {
        seed = Random.Range(0, 99999);
        Intialize_mapSpecialRoom();
        enteranceWallList = GameObject.Find("EnteranceWallList");
        RoomList = GameObject.Find("RoomList");
        Random.InitState(seed);
        _mapPoint = new Dictionary<Vector3Int, int>();
        mapPoint = new List<Vector3Int>();
        _centerPoint = new List<Vector3Int>();
        mapSize = new List<MapSize>();
        mapCenterPoint = new List<Vector3Int>();
        DrawMap();
        OpenBoxCollider();
    }
    private void Intialize_mapSpecialRoom()
    {
        //初始化所有种类的specialRoom
        SpecialRoomInfor = new List<SpecialRoomItem>();
        SpecialRoomNumber = new List<int>();
        mapSpecialRoom = new List<string>();
        mapSpecialRoom.Add("Wishing Pool");
        mapSpecialRoom.Add("Trader");
        mapSpecialRoom.Add("Treasure");
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DrawMap();
        }
        //打开房门
        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenBoxCollider();
        }
        //关闭房门
        if (Input.GetKeyDown(KeyCode.C))
        {
            CloseBoxCollider();
        }
    }
    //返回最后房间元素
    private MapSize FindFinalroom()
    {
        MapSize max = mapSize[0];
        foreach (MapSize item in mapSize)
        {
            if ((max.center.x + max.center.y) < (item.center.x + item.center.y))
            {
                max = item;
            }
        }
        return max;
    }
    private MapSize FindInitialroom()
    {
        MapSize min = mapSize[0];
        foreach (MapSize item in mapSize)
        {
            if ((min.center.x + min.center.y) > (item.center.x + item.center.y))
            {
                min = item;
            }
        }
        return min;

    }
    public void CreatePortal()
    {
        MapSize finalRoom = FindFinalroom();
        Instantiate(Portal, finalRoom.center, Quaternion.identity);
    }

    /// <summary>
    /// 画出地图
    /// </summary>
    private void DrawMap()
    {
        //清空上一次生成的地图
        _mapPoint.Clear();
        mapPoint.Clear();
        _centerPoint.Clear();
        tilemap.ClearAllTiles();
        mapSize.Clear();
        mapCenterPoint.Clear();
        var map = GetRoomMap(mapMaxW, mapMaxH, mapCount);


        for (var i = 0; i < mapMaxH; i++)
        {
            for (var j = 0; j < mapMaxW; j++)
            {
                if (map[j, i] != 1) continue;
                //乘以间距算出中心点坐标
                var centerPoint = new Vector3Int(j * distance, i * distance, 0);
                //存入中心点数组
                _centerPoint.Add(centerPoint);
                //画出房间
                DrawRoom(centerPoint.x, centerPoint.y, _centerPoint.Count);
            }
        }
        DrawRoad();
        DrawWall();
        DrawDoor();
        DrawSpecialRoom();
        CreatePortal();
        CreateObstacle();
        CreateEnterTrigger();
    }
    public void CreateObstacle()
    {
        MapSize finalRoom = FindFinalroom();
        MapSize initialRoom = FindInitialroom();
        foreach (var value in mapSize)
        {
            //判断特殊房间不生成障碍物
            if (!(value.roomNumber == initialRoom.roomNumber) && !(value.roomNumber == finalRoom.roomNumber) && !(SpecialRoomNumber.Contains(value.roomNumber)))
            {
                //生成障碍物品
                int WallCount = Random.Range(roomDecoratorMin, roomDecoratorMax + 1);//随机生成障碍物个数的范围
                for (int i = 0; i < WallCount; i++)
                {
                    //随机取得位置
                    int positionIndex = Random.Range(0, value.mapPoint.Count);
                    Vector3Int pos = value.mapPoint[positionIndex] + new Vector3Int(1, 1, 0);
                    Vector3 pos_float=new Vector3((float)(pos.x+0.5),(float)(pos.y+0.5),(float)(pos.z));
                    value.mapPoint.RemoveAt(positionIndex);
                    //随机取得障碍物
                    int WallIndex = Random.Range(0, WallArray.Length);
                    GameObject go = GameObject.Instantiate(WallArray[WallIndex], pos_float, Quaternion.identity) as GameObject;
                    go.transform.parent = DecoratorList.transform;
                }
            }
        }
    }
    public void DrawSpecialRoom()
    {
        var specialRoomNumber = Random.Range(specialroomMin, specialroomMax);
        int[] checkRepeate = new int[mapCount];
        //存储specialRoom
        int i = 0;
        while (i < specialRoomNumber)
        {
            MapSize finalRoom = FindFinalroom();
            MapSize initialRoom = FindInitialroom();
            //1:房间类别 2:房间号
            int roomIndex = 1;
            print("Final Room " + finalRoom.roomNumber);
            print("Initial Room " + initialRoom.roomNumber);

            while (roomIndex == finalRoom.roomNumber || roomIndex == initialRoom.roomNumber)
            {
                roomIndex = Random.Range(1, mapCount);
            }

            var roomType = Random.Range(0,mapSpecialRoom.Count);
            if (checkRepeate[roomIndex] == 0)
            {
                checkRepeate[roomIndex] = 1;
                foreach (var value in mapSize)
                {
                    if (value.roomNumber == roomIndex)
                    {
                        print("Room Index is " + roomIndex);
                        print("Room Type is " + mapSpecialRoom[roomType]);
                        SpecialRoomInfor.Add(new SpecialRoomItem(roomIndex, mapSpecialRoom[roomType], value));
                        SpecialRoomNumber.Add(roomIndex);
                    }
                }
                i++;
            }
        }
        //画出specialRoom
        foreach (var specialRoom in SpecialRoomInfor)
        {

            if(specialRoom.room=="Wishing Pool"){
                GameObject Pools = Instantiate(WishingPool, specialRoom.mapSize.center, Quaternion.identity);
                Pools.transform.parent = PoolList.transform;

            }else if(specialRoom.room =="Trader"){
                GameObject Store = Instantiate(Trader, specialRoom.mapSize.center-new Vector3Int(0,-2,0), Quaternion.identity);
                GameObject ShowTableL= Instantiate(Table, specialRoom.mapSize.center+new Vector3Int(-2,-1,0), Quaternion.identity);
                GameObject ShowTableM = Instantiate(Table, specialRoom.mapSize.center+new Vector3Int(0,-1,0), Quaternion.identity);
                GameObject ShowTableR = Instantiate(Table, specialRoom.mapSize.center+new Vector3Int(2,-1,0), Quaternion.identity);
                Store.transform.parent = StoreList.transform;
                ShowTableL.transform.parent=TableList.transform;
                ShowTableM.transform.parent=TableList.transform;
                ShowTableR.transform.parent=TableList.transform;
            }else if (specialRoom.room == "Treasure"){

                
                Vector3 position = new Vector3((float)(specialRoom.mapSize.center.x + 0.5), (float)(specialRoom.mapSize.center.y + 0.5), (float)(specialRoom.mapSize.center.z));

                GameObject TreasuryBox = Instantiate(Treasure, position, Quaternion.identity);
                TreasuryBox.transform.parent = TreasureList.transform;

                
            }
            //把specialRoom放到正确的位置
            //print(SpecialRoomInfor[j].roomNumber);
            //print(((wishingPond)SpecialRoomInfor[j].room).expenditure);
        }
    }

    /// <summary>
    /// 画出房间
    /// </summary>
    /// <param name="roomX">中心点坐标 X</param>
    /// <param name="roomY">中心点坐标 Y</param>
    private void DrawRoom(int roomX, int roomY, int roomNumber)
    {
        mapPoint.Clear();
        var room = RandomRoom(roomMaxW, roomMaxH, roomMinW, roomMinH, out var width, out var height, roomX, roomY, roomNumber);
        var point = new Vector2Int { x = roomX - (width >> 1), y = roomY - (height >> 1) };
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                var nowPoint = new Vector3Int(point.x + i, point.y + j, 0);

                //判断 tile 类型，并且存入 _mapPoint
                if (room[i, j] == 1)
                {
                    tilemap.SetTile(nowPoint, floor);
                    _mapPoint.Add(nowPoint, 1);
                    if (i != 0 && i != 1 && i != width - 1 && j != 0 && j != 1 && j != height - 1)
                    {
                        mapPoint.Add(nowPoint);
                    }
                }
            }
        }
        var center = new Vector3Int(roomX, roomY, 0);
        var roomDetail = new int[width, height];
        MapSize mapsize = new MapSize(height, width, center, roomDetail, roomNumber, new List<Vector3Int>(mapPoint));
        mapSize.Add(mapsize);

    }

    /// <summary>
    /// 给房间之间画出 3 格宽的路
    /// </summary>
    private void DrawRoad()
    {
        // X 轴连通房间
        foreach (var i in _centerPoint)
        {
            var nextPint = new Vector3Int(i.x + distance, i.y, i.z);
            if (!_centerPoint.Contains(nextPint)) continue;
            for (var j = 0; j < distance; j++)
            {
                if (_mapPoint.ContainsKey(new Vector3Int(i.x + j, i.y, i.z))) continue;
                //画出 3 格宽的路
                tilemap.SetTile(new Vector3Int(i.x + j, i.y, i.z), floor);
                tilemap.SetTile(new Vector3Int(i.x + j, i.y + 1, i.z), floor);
                tilemap.SetTile(new Vector3Int(i.x + j, i.y - 1, i.z), floor);
                //存入 _mapPoint
                _mapPoint.Add(new Vector3Int(i.x + j, i.y, i.z), 1);
                _mapPoint.Add(new Vector3Int(i.x + j, i.y + 1, i.z), 1);
                _mapPoint.Add(new Vector3Int(i.x + j, i.y - 1, i.z), 1);
            }
        }

        // Y 轴连通房间
        foreach (var i in _centerPoint)
        {

            var nextPint = new Vector3Int(i.x, i.y + distance, i.z);
            if (!_centerPoint.Contains(nextPint)) continue;
            for (var j = 0; j < distance; j++)
            {
                if (_mapPoint.ContainsKey(new Vector3Int(i.x, i.y + j, i.z))) continue;
                //画出 3 格宽的路
                tilemap.SetTile(new Vector3Int(i.x, i.y + j, i.z), floor);
                tilemap.SetTile(new Vector3Int(i.x + 1, i.y + j, i.z), floor);
                tilemap.SetTile(new Vector3Int(i.x - 1, i.y + j, i.z), floor);
                //存入 _mapPoint
                _mapPoint.Add(new Vector3Int(i.x, i.y + j, i.z), 1);
                _mapPoint.Add(new Vector3Int(i.x + 1, i.y + j, i.z), 1);
                _mapPoint.Add(new Vector3Int(i.x - 1, i.y + j, i.z), 1);
            }
        }

    }

    /// <summary>
    /// 遍历所有有物体的格子，给周围画出墙壁
    /// </summary>
    private void DrawWall()
    {
        var leftPoint = new Vector3Int(-1, 0, 0);
        var rightPoint = new Vector3Int(1, 0, 0);
        var upPoint = new Vector3Int(0, 1, 0);
        var downPoint = new Vector3Int(0, -1, 0);
        var leftTopCorner = new Vector3Int(-1, 1, 0);
        var rightTopCorner = new Vector3Int(1, 1, 0);
        var leftDownCorner = new Vector3Int(-1, -1, 0);
        var rightDownCorner = new Vector3Int(1, -1, 0);
        var tempMapPoint = new List<Vector3Int>();



        //画出墙壁
        foreach (var i in _mapPoint.Keys)
        {
            if (!_mapPoint.ContainsKey(i + leftPoint))
            {
                tilemap.SetTile(i + leftPoint, wallLR);
                tempMapPoint.Add(i + leftPoint);
            }
            if (!_mapPoint.ContainsKey(i + rightPoint))
            {
                tilemap.SetTile(i + rightPoint, wallLR);
                tempMapPoint.Add(i + rightPoint);
            }
            if (!_mapPoint.ContainsKey(i + upPoint))
            {
                tilemap.SetTile(i + upPoint, wall);
                tempMapPoint.Add(i + upPoint);
            }
            if (!_mapPoint.ContainsKey(i + downPoint))
            {
                tilemap.SetTile(i + downPoint, wall);
                tempMapPoint.Add(i + downPoint);
            }

            //周围

            if (!_mapPoint.ContainsKey(i + leftTopCorner))
            {
                tilemapWall.SetTile(i + leftTopCorner, wall);
                tempMapPoint.Add(i + leftTopCorner);
            }
            if (!_mapPoint.ContainsKey(i + rightTopCorner))
            {
                tilemapWall.SetTile(i + rightTopCorner, wall);
                tempMapPoint.Add(i + rightTopCorner);
            }
            if (!_mapPoint.ContainsKey(i + leftDownCorner))
            {
                tilemapWall.SetTile(i + leftDownCorner, wall);
                tempMapPoint.Add(i + leftDownCorner);
            }
            if (!_mapPoint.ContainsKey(i + rightDownCorner))
            {
                tilemapWall.SetTile(i + rightDownCorner, wall);
                tempMapPoint.Add(i + rightDownCorner);
            }

            //需要修改
            //添加EnemyArea
            if (!_mapPoint.ContainsKey(i + rightDownCorner) && !_mapPoint.ContainsKey(i + leftDownCorner) && !_mapPoint.ContainsKey(i + leftTopCorner))
            {
                GameObject enemyAreaObject = Instantiate(EnemyArea, i, Quaternion.identity);
                enemyAreaObject.transform.SetParent(RoomList.transform);
            }


        }
        //存入 _mapPoint 
        foreach (var value in tempMapPoint.Where(value => !_mapPoint.ContainsKey(value)))
        {
            _mapPoint.Add(value, 0);
        }
    }
    public void OpenBoxCollider()
    {
        for (int i = 0; i < enteranceWallList.transform.childCount; i++)
        {
            Transform Door = enteranceWallList.transform.GetChild(i);
            Door.GetComponent<BoxCollider2D>().isTrigger = true;
            tilemap.SetTile(new Vector3Int((int)Door.position.x, (int)Door.position.y, 0), OpenDoor);
            //tilemapWall.SetTile(new Vector3Int((int)Door.position.x, (int)Door.position.y, 0), OpenDoor);
        }
    }
    public void CloseBoxCollider()
    {
        for (int i = 0; i < enteranceWallList.transform.childCount; i++)
        {
            Transform Door = enteranceWallList.transform.GetChild(i);
            Door.GetComponent<BoxCollider2D>().isTrigger = false;
            tilemap.SetTile(new Vector3Int((int)Door.position.x, (int)Door.position.y, 0), door);
            //tilemapWall.SetTile(new Vector3Int((int)Door.position.x, (int)Door.position.y, 0), door);
        }
    }

    private void DrawDoor()
    {
        foreach (var value in mapSize)
        {

            //跳过第一关
            if (value.center == new Vector3Int(0, 0, 0)) continue;

            Vector3Int DoorXRight = value.center + new Vector3Int(value.width >> 1, 0, 0);
            Vector3Int DoorXLeft = value.center - new Vector3Int(value.width >> 1, 0, 0);
            Vector3Int DoorYUp = value.center + new Vector3Int(0, value.height >> 1, 0);
            Vector3Int DoorYDown = value.center - new Vector3Int(0, value.height >> 1, 0);

            //X轴的右门
            if (tilemap.GetTile(new Vector3Int(DoorXRight.x + 1, DoorXRight.y, DoorXRight.z)) == floor)
            {

                tilemap.SetTile(new Vector3Int(DoorXRight.x + 1, DoorXRight.y + 1, DoorXRight.z), door);
                tilemap.SetTile(new Vector3Int(DoorXRight.x + 1, DoorXRight.y, DoorXRight.z), door);
                tilemap.SetTile(new Vector3Int(DoorXRight.x + 1, DoorXRight.y - 1, DoorXRight.z), door);

                GameObject wallobject0 = Instantiate(Wall, new Vector3Int(DoorXRight.x + 1, DoorXRight.y + 1, DoorXRight.z), Quaternion.identity);
                GameObject wallobject1 = Instantiate(Wall, new Vector3Int(DoorXRight.x + 1, DoorXRight.y, DoorXRight.z), Quaternion.identity);
                GameObject wallobject2 = Instantiate(Wall, new Vector3Int(DoorXRight.x + 1, DoorXRight.y - 1, DoorXRight.z), Quaternion.identity);

                wallobject0.transform.parent = enteranceWallList.transform;
                wallobject1.transform.parent = enteranceWallList.transform;
                wallobject2.transform.parent = enteranceWallList.transform;


            }
            //X轴的左门
            if (tilemap.GetTile(new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y, DoorXLeft.z)) == floor || tilemap.GetTile(new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y, DoorXLeft.z)) == door || tilemap.GetTile(new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y, DoorXLeft.z)) == Realdoor)
            {

                tilemap.SetTile(new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y + 1, DoorXLeft.z), door);
                tilemap.SetTile(new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y, DoorXLeft.z), door);
                tilemap.SetTile(new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y - 1, DoorXLeft.z), door);

                tilemap.SetTile(new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y + 1, DoorXLeft.z), Realdoor);
                tilemap.SetTile(new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y, DoorXLeft.z), Realdoor);
                tilemap.SetTile(new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y - 1, DoorXLeft.z), Realdoor);

                GameObject wallobject0 = Instantiate(Wall, new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y + 1, DoorXLeft.z), Quaternion.identity);
                GameObject wallobject1 = Instantiate(Wall, new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y, DoorXLeft.z), Quaternion.identity);
                GameObject wallobject2 = Instantiate(Wall, new Vector3Int(DoorXLeft.x - 1, DoorXLeft.y - 1, DoorXLeft.z), Quaternion.identity);

                wallobject0.transform.parent = enteranceWallList.transform;
                wallobject1.transform.parent = enteranceWallList.transform;
                wallobject2.transform.parent = enteranceWallList.transform;
            }
            //Y轴的上门
            if (tilemap.GetTile(new Vector3Int(DoorYUp.x, DoorYUp.y + 1, DoorYUp.z)) == floor || tilemap.GetTile(new Vector3Int(DoorYUp.x, DoorYUp.y + 1, DoorYUp.z)) == door || tilemap.GetTile(new Vector3Int(DoorYUp.x, DoorYUp.y + 1, DoorYUp.z)) == Realdoor)
            {

                tilemap.SetTile(new Vector3Int(DoorYUp.x + 1, DoorYUp.y + 1, DoorYUp.z), door);
                tilemap.SetTile(new Vector3Int(DoorYUp.x, DoorYUp.y + 1, DoorYUp.z), door);
                tilemap.SetTile(new Vector3Int(DoorYUp.x - 1, DoorYUp.y + 1, DoorYUp.z), door);

                tilemap.SetTile(new Vector3Int(DoorYUp.x + 1, DoorYUp.y + 1, DoorYUp.z), Realdoor);
                tilemap.SetTile(new Vector3Int(DoorYUp.x, DoorYUp.y + 1, DoorYUp.z), Realdoor);
                tilemap.SetTile(new Vector3Int(DoorYUp.x - 1, DoorYUp.y + 1, DoorYUp.z), Realdoor);

                GameObject wallobject0 = Instantiate(Wall, new Vector3Int(DoorYUp.x + 1, DoorYUp.y + 1, DoorYUp.z), Quaternion.identity);
                GameObject wallobject1 = Instantiate(Wall, new Vector3Int(DoorYUp.x, DoorYUp.y + 1, DoorYUp.z), Quaternion.identity);
                GameObject wallobject2 = Instantiate(Wall, new Vector3Int(DoorYUp.x - 1, DoorYUp.y + 1, DoorYUp.z), Quaternion.identity);

                wallobject0.transform.parent = enteranceWallList.transform;
                wallobject1.transform.parent = enteranceWallList.transform;
                wallobject2.transform.parent = enteranceWallList.transform;
            }
            //Y轴的下门
            if (tilemap.GetTile(new Vector3Int(DoorYDown.x, DoorYDown.y - 1, DoorYDown.z)) == floor || tilemap.GetTile(new Vector3Int(DoorYDown.x, DoorYDown.y - 1, DoorYDown.z)) == door || tilemap.GetTile(new Vector3Int(DoorYDown.x, DoorYDown.y - 1, DoorYDown.z)) == Realdoor)
            {
                tilemap.SetTile(new Vector3Int(DoorYDown.x + 1, DoorYDown.y - 1, DoorYDown.z), door);
                tilemap.SetTile(new Vector3Int(DoorYDown.x, DoorYDown.y - 1, DoorYDown.z), door);
                tilemap.SetTile(new Vector3Int(DoorYDown.x - 1, DoorYDown.y - 1, DoorYDown.z), door);

                tilemap.SetTile(new Vector3Int(DoorYDown.x + 1, DoorYDown.y - 1, DoorYDown.z), Realdoor);
                tilemap.SetTile(new Vector3Int(DoorYDown.x, DoorYDown.y - 1, DoorYDown.z), Realdoor);
                tilemap.SetTile(new Vector3Int(DoorYDown.x - 1, DoorYDown.y - 1, DoorYDown.z), Realdoor);

                GameObject wallobject0 = Instantiate(Wall, new Vector3Int(DoorYDown.x + 1, DoorYDown.y - 1, DoorYDown.z), Quaternion.identity);
                GameObject wallobject1 = Instantiate(Wall, new Vector3Int(DoorYDown.x, DoorYDown.y - 1, DoorYDown.z), Quaternion.identity);
                GameObject wallobject2 = Instantiate(Wall, new Vector3Int(DoorYDown.x - 1, DoorYDown.y - 1, DoorYDown.z), Quaternion.identity);

                wallobject0.transform.parent = enteranceWallList.transform;
                wallobject1.transform.parent = enteranceWallList.transform;
                wallobject2.transform.parent = enteranceWallList.transform;
            }
        }
    }

    /// <summary>
    /// 返回一个二维数组表示房间
    /// </summary>
    /// <param name="maxW">最大宽度</param>
    /// <param name="maxH">最大高度</param>
    /// <param name="minW">最小宽度</param>
    /// <param name="minH">最小高度</param>
    /// <param name="width">生成的房间宽度</param>
    /// <param name="height">生成的房间高度</param>
    /// <returns></returns>
    private int[,] RandomRoom(int maxW, int maxH, int minW, int minH, out int width, out int height, int roomX, int roomY, int roomNumber)
    {
        width = GetOddNumber(minW, maxW);
        height = GetOddNumber(minH, maxH);

        var room = new int[width, height];


        //方便以后扩展使用了二维数组，这里为了演示方便对房间内生成其他物体
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                room[i, j] = 1;
            }
        }


        return room;
    }

    /// <summary>
    /// 取一个指定范围内的奇数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns></returns>
    private int GetOddNumber(int min, int max)
    {
        while (true)
        {
            var temp = Random.Range(min, max);

            if ((temp & 1) != 1) continue;
            return temp;
        }
    }

    /// <summary>
    /// 返回一个有房间的地图 ，使用二维int数组,1 代表有房间
    /// </summary>
    /// <param name="mapW">地图的宽度</param>
    /// <param name="mapH">地图的高度</param>
    /// <param name="roomCount">生成的房间总数</param>
    /// <returns></returns>
    private int[,] GetRoomMap(int mapW, int mapH, int roomCount)
    {
        //第一个房间的坐标点
        var nowPoint = Vector2Int.zero;
        //当前生成的房间数
        var mCount = 1;
        //当前地图
        var map = new int[mapW, mapH];

        map[nowPoint.x, nowPoint.y] = 1;

        while (mCount < roomCount)
        {
            nowPoint = GetNextPoint(nowPoint, mapW, mapH);
            if (map[nowPoint.x, nowPoint.y] == 1) continue;
            map[nowPoint.x, nowPoint.y] = 1;
            mCount++;
        }
        //存储map
        return map;
    }

    /// <summary>
    /// 获取下一个房间位置
    /// </summary>
    /// <param name="nowPoint">现在房间的位置</param>
    /// <param name="maxW">一行最多有几个房间</param>
    /// <param name="maxH">一列最多有几个房间</param>
    /// <returns></returns>
    private Vector2Int GetNextPoint(Vector2Int nowPoint, int maxW, int maxH)
    {
        while (true)
        {
            var mNowPoint = nowPoint;

            switch (Random.Range(0, 4))
            {
                case 0:
                    mNowPoint.x += 1;
                    break;
                case 1:
                    mNowPoint.y += 1;
                    break;
                case 2:
                    mNowPoint.x -= 1;
                    break;
                default:
                    mNowPoint.y -= 1;
                    break;
            }

            if (mNowPoint.x >= 0 && mNowPoint.y >= 0 && mNowPoint.x < maxW && mNowPoint.y < maxH)
            {
                return mNowPoint;
            }
        }
    }

    private void CreateEnterTrigger()
    {

        MapSize finalRoom = FindFinalroom();
        MapSize initialRoom = FindInitialroom();
        GameObject[] Rooms = GameObject.FindGameObjectsWithTag("Room");

        for (int i = 0; i < mapSize.Count; i++)
        {
            if (!(mapSize[i].roomNumber == initialRoom.roomNumber) && !(mapSize[i].roomNumber == finalRoom.roomNumber) && !(SpecialRoomNumber.Contains(mapSize[i].roomNumber)))
            {
                GameObject EnteranceTrigger = Instantiate(TriggerObject, new Vector3Int(mapSize[i].center.x, mapSize[i].center.y, 0), Quaternion.identity);
                EnteranceTrigger.GetComponent<BoxCollider2D>().size = new Vector2(mapSize[i].width, mapSize[i].height);
                EnteranceTrigger.transform.SetParent(Rooms[i].transform);
                EnteranceTrigger.GetComponentInParent<EnemyArea>().width = mapSize[i].width;
                EnteranceTrigger.GetComponentInParent<EnemyArea>().height = mapSize[i].height;
            }
        }
    }
}