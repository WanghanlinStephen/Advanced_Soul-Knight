using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    void Attack();
    void BeAttacked(float damage, Vector3 impactForce);
    float GetInfo(string type);
    void DropDown(Transform item);
}
