using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoGenerator : ObjectInfoGenerator
{
    public override string InfoGenerate()
    {
        IWeapon here = this.gameObject.GetComponentInChildren<IWeapon>();
        string write = "";
        write += ("伤害：" + here.GetInfo("伤害") + "\n");
        write += ("冷却：" + here.GetInfo("冷却") + "\n");
        write += ("击退：" + here.GetInfo("击退") + "\n");
        write += ("费用：" + here.GetInfo("费用") + "\n");
        Debug.Log(write);
        return write;
    }
}
