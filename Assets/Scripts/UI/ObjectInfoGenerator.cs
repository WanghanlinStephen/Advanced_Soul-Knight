using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ObjectInfoGenerator : MonoBehaviour
{
    public GameObject InfoPre, HolderPre;  // 信息栏预制体
    protected Canvas UIHolder;  // 游戏物体上的画布
    protected GameObject info;  // 保留信息栏的GameObject形式，方便隐藏
    protected Text infoText;
    protected RectTransform rectBloodPos; // 跟踪的对象
    void Start()
    {
        //UIHolder = this.gameObject.GetComponentInChildren<Canvas>();
        UIHolder = Instantiate(HolderPre).GetComponent<Canvas>();
        UIHolder.transform.name = "InfoUIHolder";
        UIHolder.transform.SetParent(this.transform);
        info = Instantiate(InfoPre);
        info.transform.name = "InfoText";
        info.SetActive(false);
        infoText = info.GetComponent<Text>();
        infoText.transform.SetParent(UIHolder.transform);   // 将infoText绑定到UIHolder下
        rectBloodPos = infoText.GetComponent<RectTransform>();
        infoText.text = InfoGenerate();
    }

    // ==============================================信息填写函数========================================================
    public abstract string InfoGenerate();

    // ==============================================信息显示、隐藏函数========================================================
    public void ShowInfo()
    {
        info.SetActive(true);
    }
    public void HideInfo()
    {
        info.SetActive(false);
    }

    // ==============================================跟踪函数========================================================
    protected void Follow()
    {
        Vector2 vec2 = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
        rectBloodPos.transform.position = new Vector2(vec2.x, vec2.y + 40); // 这样写没有跟随没有延迟
    }

    // ==============================================物品位置检查函数========================================================
    protected bool Scan()
    {
        if (this.gameObject.transform.parent != GameManager.itemList)
        {
            info.SetActive(false);
            return false;
        }
        else
        {
            return true;
        }
    }


    void Update()
    {
        if (Scan())
        {
            Follow();
        }
    }
}
