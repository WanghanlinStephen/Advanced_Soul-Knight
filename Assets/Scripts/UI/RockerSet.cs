using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockerSet : MonoBehaviour
{
    // RockerSet是摇杆RockerSet的控制器，负责监测触摸事件，并随之定位摇杆盘和摇杆
    // RockerSet也负责向外输出摇杆位置信息
    //==========================================变量============================================
    // 设定触摸事件开始时出现摇杆盘的允许范围
    // 值范围：-0.5~0.5，表示百分比
    [Range(-0.5f, 0.5f)]
    public float panelAllowedPositionStartX, panelAllowedPositionEndX, 
        panelAllowedPositionStartY, panelAllowedPositionEndY;
    //public float panelListenRadius;
    public float obstructionOnDistance; // 阻力开始距离
    public float specialOnDistance;     // 特殊技能开始距离
    [Range(0, 1)]
    public float obstruction;           // 阻力强度(越小越强，0~1)

    private Transform panel, rocker;
    // 根据屏幕大小计算好的允许范围
    private float panelAllowedPositionStartMappedX, panelAllowedPositionEndMappedX, 
        panelAllowedPositionStartMappedY, panelAllowedPositionEndMappedY;
    // 触控相关变量
    private int fingerId;
    private bool touched;
    void Start()
    {
        this.gameObject.SetActive(false);
        panel = this.transform.Find("Panel");
        rocker = this.transform.Find("Rocker");
        float height = Screen.height;
        float width = Screen.width;
        panelAllowedPositionStartMappedX = (panelAllowedPositionStartX + 0.5f) * width;
        panelAllowedPositionEndMappedX = (panelAllowedPositionEndX + 0.5f) * width;
        panelAllowedPositionStartMappedY = (panelAllowedPositionStartY + 0.5f) * height;
        panelAllowedPositionEndMappedY = (panelAllowedPositionEndY + 0.5f) * height;


        fingerId = -1;
        touched = false;
    }

    // ======================================位置转换函数=========================================
    private Vector2 AbsoluteToRelative(Vector2 absolutePos)
    {
        return new Vector2(absolutePos.x - panel.position.x, absolutePos.y - panel.position.y);
    }
    private Vector2 RelativeToAbsolute(Vector2 relativePos)
    {
        return new Vector2(relativePos.x + panel.position.x, relativePos.y + panel.position.y);
    }
    private Vector2 PosMap(Vector2 touchPos)
    {
        // 开方投射位置
        // 用于施加一个模拟的回拉力
        Vector2 relativePos = AbsoluteToRelative(touchPos);
        Vector2 center = new Vector2(0, 0);
        float exceedDistance = Vector2.Distance(relativePos, center) - obstructionOnDistance;
        if (exceedDistance <= 0)
        {
            return touchPos;
        }
        else
        {
            float mappingRatio = ((float)Math.Pow((double)exceedDistance, obstruction) + obstructionOnDistance)
                / (exceedDistance + obstructionOnDistance);
            return RelativeToAbsolute(new Vector2(relativePos.x * mappingRatio, relativePos.y * mappingRatio));
        }
    }

    // ======================================触点判断函数=========================================
    public bool InThisRange(Vector2 touchPos)
    {
        // 用于判断触点是否在该摇杆的感应区内
        if (touchPos.x <= panelAllowedPositionEndMappedX 
            && touchPos.x >= panelAllowedPositionStartMappedX
            && touchPos.y <= panelAllowedPositionEndMappedY
            && touchPos.y >= panelAllowedPositionStartMappedY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 目前弃用
    //public bool InPanelRange(Vector2 touchPos)
    //{
    //    // 判断触点是否在距盘心一定距离内
    //    return Vector2.Distance(touchPos, panel.position) < panelListenRadius;
    //}

    // ======================================事件=========================================
    public void OnPointerDown(Touch touch)
    {
        //由TouchListener监测到该摇杆感应区域内的触点后调用
        this.gameObject.SetActive(true);    // 显示摇杆
        this.fingerId = touch.fingerId;     // 记录触点fingerId，用于结束此次触摸前的跟踪
        Vector2 at = touch.position;
        if (!touched)
        {
            // 该判断用于防止两边摇杆OnPointerDown的联动
            this.gameObject.transform.position = at;
        }
        this.touched = true;                // 更新touched状态，Update中的代码块开始执行
    }

    public void OnDrag(Vector2 at)
    {
        // 投射触点和rocker位置
        rocker.position = PosMap(at);
    }

    public void OnEndDrag()
    {
        this.gameObject.SetActive(false);   // 隐藏摇杆
        rocker.position = panel.position;   // rocker归位
        touched = false;                    // 更新touched状态，Update中的代码块停止执行
        fingerId = -1;                      // 消除fingerId记录
    }

    // ======================================对接=========================================
    public Vector2 RockerRelativePos()
    {
        // 直接从成员变量获取rockerPos进行转换
        // 没有归一化
        return new Vector2(rocker.position.x - panel.position.x, rocker.position.y - panel.position.y);
    }

    public bool SpecialOn()
    {
        // 判断特殊技能是否启用
        return Vector2.Distance(RockerRelativePos(), Vector2.zero) > specialOnDistance;
    }

    public Vector2 RestrictedRockerRelativePos()
    {
        // 将输出范围限定在阻力开始位置
        return new Vector2(Mathf.Clamp(rocker.position.x - panel.position.x, -obstructionOnDistance, obstructionOnDistance),
            Mathf.Clamp(rocker.position.y - panel.position.y, -obstructionOnDistance, obstructionOnDistance));
    }

    // ======================================监测=========================================
    void Update()
    {
        if (touched)
        {
            // 因为没有直接通过fingerId获取触摸事件的方法，遍历寻找此次触摸
            foreach (Touch t in Input.touches)
            {
                if (t.fingerId == this.fingerId)
                {
                    OnDrag(t.position);
                    if(t.phase == TouchPhase.Ended)
                    {
                        OnEndDrag();
                    }
                }
            }
        }
    }
}
