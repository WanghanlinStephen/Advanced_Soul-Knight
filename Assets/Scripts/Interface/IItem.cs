using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IItem
{
    void Drop();
    void Drop(float coolingTime);
    void PickBy(Transform t);
}
