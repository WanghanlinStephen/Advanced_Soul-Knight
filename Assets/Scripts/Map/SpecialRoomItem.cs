using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRoomItem
{
    public int roomNumber;
    public string room;
    public MapSize mapSize;

    public SpecialRoomItem(int roomNumber,string room,MapSize mapSize){
        this.roomNumber=roomNumber;
        this.room=room;
        this.mapSize=mapSize;
    }
}