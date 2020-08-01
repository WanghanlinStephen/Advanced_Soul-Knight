using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterUIManager : MonoBehaviour
{
    // CharacterUIManager是PlayerUIManager和EnemyUIManager的父类，方便更改和维护

    public GameObject BarPre, HolderPre;   // 血条预制体
    protected GameObject hb;  // 保留血条的GameObject形式，方便隐藏
    protected Canvas UIHolder;    // 被赋予人物内的Canvas，用于装载血条
    protected RectTransform rectBloodPos; // 跟踪的对象
    public Slider HealthBar;
    // ==============================================逻辑用变量=========================================================
    public float setFlashCount;   // 设定血条消失帧数
    protected float flashCounter;  // 计时器
    protected float lastCurAmount;    // 核对是否血量有变化

    // ==============================================开始方法===========================================================
    protected void Start()
    {
        UIHolder = Instantiate(HolderPre).GetComponent<Canvas>();
        UIHolder.transform.name = "UIHolder";
        UIHolder.transform.SetParent(this.transform);
        //UIHolder = this.gameObject.GetComponentInChildren<Canvas>();
        hb = Instantiate(BarPre);
        hb.transform.name = "SmallHealthBar";
        HealthBar = hb.GetComponent<Slider>();
        HealthBar.transform.SetParent(UIHolder.transform);
        rectBloodPos = HealthBar.GetComponent<RectTransform>();

        flashCounter = 0;
        lastCurAmount = 0;
    }


    // ==============================================判断是否在相机视野内================================================
    public bool IsInView(Vector3 worldPos)
    {
        //Transform camTransform = Camera.main.transform;
        Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);

        if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            return true;
        else
            return false;
    }

    // ==============================================跟踪函数========================================================
    protected void Follow()
    {
        Vector2 vec2 = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);

        if (IsInView(transform.position))
        {
            //rectBloodPos.gameObject.SetActive(true);
            hb.SetActive(hb.activeSelf);
            //rectBloodPos.anchoredPosition = new Vector2(vec2.x - Screen.width / 2 + 0, vec2.y - Screen.height / 2 + 60);
            rectBloodPos.transform.position = new Vector2(vec2.x, vec2.y + 40); // 这样写没有跟随没有延迟
        }
        //else rectBloodPos.gameObject.SetActive(false);
        else
        {
            hb.SetActive(false);
        }
    }

    // ==============================================显示判断函数========================================================
    protected bool IsAttacked(float curAmount)
    {
        return lastCurAmount != curAmount;
    }

    // true 为隐藏，false 为显示
    protected bool IsFull(float curAmount, float maxAmount)
    {
        return curAmount >= maxAmount;
    }

    protected bool IsEmpty(float curAmount)
    {
        return curAmount <= 0;
    }

    protected bool IsTime(float curAmount)
    {
        if (IsAttacked(curAmount))
        {
            lastCurAmount = curAmount;
            flashCounter = 0;
            return false;
        }
        else
        {
            lastCurAmount = curAmount;
            if (flashCounter < setFlashCount)
            {
                flashCounter += Time.deltaTime;
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    // ==============================================更新函数========================================================
    protected bool BasicSliderBarUpdate(float curAmount, float maxAmount)
    {
        // 更新血条
        DisplayContainerManager.instance.UpdateHealthBar(HealthBar, curAmount, maxAmount);
        // 确定是否显示
        if (IsFull(curAmount, maxAmount))
        {
            hb.SetActive(false);
            return false;
        }
        else if (IsEmpty(curAmount))
        {
            hb.SetActive(false);
            return false;
        }
        else
        {
            if (IsTime(curAmount))
            {
                hb.SetActive(false);
                return false;
            }
            else
            {
                hb.SetActive(true);
                return true;
            }
        }
    }
    public abstract bool SliderBarUpdate();

    // Update is called once per frame
    protected void Update()
    {
        if (SliderBarUpdate())
        {
            Follow();
        }
    }
}
