using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    public float Attack()
    {
        return -1f;
    }

    public float GetInfo(string type)
    {
        return 0f;
    }

    public bool IsMelee()
    {
        return false;
    }

    public void SetInfo(string type, float value)
    {
        
    }
}
