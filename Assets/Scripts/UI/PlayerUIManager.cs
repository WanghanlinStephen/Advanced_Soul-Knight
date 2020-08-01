using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : CharacterUIManager
{
    private Image MainHealthBar, MainEnergyBar; // 大血条、大能量条
    private Text MainCoinText;  // 大金币
    // ==============================================开始方法===========================================================
    new void Start()
    {
        base.Start();
        MainHealthBar = GameObject.Find("MainHealthContainer").transform.Find("Bar").GetComponent<Image>();
        MainEnergyBar = GameObject.Find("MainEnergyContainer").transform.Find("Bar").GetComponent<Image>();
        MainCoinText = GameObject.Find("MainCoinContainer").transform.Find("Text").GetComponent<Text>();
    }
    // ==============================================更新函数========================================================
    private void MainBarUpdate()
    {
        hb.SetActive(false);
        Player here = this.gameObject.GetComponent<Player>();
        DisplayContainerManager.instance.UpdateHealthBar(MainHealthBar, here.GetInfo("HP"), here.GetInfo("MaxHP"));
        DisplayContainerManager.instance.UpdateEnergyBar(MainEnergyBar, here.GetInfo("MP"), here.GetInfo("MaxMP"));
        DisplayContainerManager.instance.UpdateCoinText(MainCoinText, here.GetInfo("Coins"));
    }

    override public bool SliderBarUpdate()
    {
        Player here = this.gameObject.GetComponent<Player>();
        //float curAmount = here.GetInfo("HP");
        //float maxAmount = here.GetInfo("MaxHP");
        return BasicSliderBarUpdate(here.GetInfo("HP"), here.GetInfo("MaxHP"));
    }

    new void Update()
    {
        if (this.tag == "Player")
        {
            MainBarUpdate();
        }
        else
        {
            base.Update();
        }
    }
}
