using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSize
{
    public int height;
    public int width;
    public Vector3Int center;
    public int[,] roomInfor;
    public int roomNumber;
    public List<Vector3Int> mapPoint;

    public MapSize(int height,int width,Vector3Int center,int[,] roomInfor,int roomNumber ,List<Vector3Int> mapPoint){
        this.height=height;
        this.width=width;
        this.center= new Vector3Int(center.x, center.y,0);
        this.roomInfor=roomInfor;
        this.roomNumber=roomNumber;
        this.mapPoint=mapPoint;
    }
}
