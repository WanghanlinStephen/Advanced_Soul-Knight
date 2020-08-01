using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIManager : CharacterUIManager
{
    // ==============================================更新函数========================================================
    override public bool SliderBarUpdate()
    {
        Enemy here = this.gameObject.GetComponent<Enemy>();
        return BasicSliderBarUpdate(here.GetInfo("HP"), here.GetInfo("MaxHP"));
    }
}