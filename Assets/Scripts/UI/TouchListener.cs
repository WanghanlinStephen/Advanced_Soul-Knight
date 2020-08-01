using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchListener : MonoBehaviour
{
    // =================================设置单例模式======================================
    public static TouchListener instance;   // 单例模式
    void Awake()
    {
        instance = this;
    }

    // ==================================下属两个摇杆控制脚本==============================
    public RockerSet movementRockerControl, skillRockerControl;
    void Start()
    {
        // 允许多点触控
        Input.multiTouchEnabled = true;
    }

    // 被EventTrigger调用
    // 判断触点位置，调用相应摇杆的OnPointerDown方法
    public void OnPointerDown()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch t in Input.touches)
            {
                if (movementRockerControl.InThisRange(t.position))
                {
                    movementRockerControl.OnPointerDown(t);
                }
                else if (skillRockerControl.InThisRange(t.position))
                {
                    skillRockerControl.OnPointerDown(t);
                }
                else
                {
                    return;
                }
            }
        }
    }
    // ==================================对外输出摇杆的位置向量==============================
    public Vector2 GetMovementRockerPos()
    {
        return movementRockerControl.RestrictedRockerRelativePos();
    }
    public Vector2 GetSkillRockerPos()
    {
        return skillRockerControl.RestrictedRockerRelativePos();
    }

    void Update()
    {

    }
}
