using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    float Attack();
    bool IsMelee();
    float GetInfo(string type);
    void SetInfo(string type, float value);
}
