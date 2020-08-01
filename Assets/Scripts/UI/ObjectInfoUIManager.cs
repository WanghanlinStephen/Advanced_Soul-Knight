using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfoUIManager : MonoBehaviour
{
    public float scanRange;
    void Start()
    {
        
    }

    // ==============================================求距离函数========================================================
    float Distance(GameObject item)
    {
        Vector2 thisPos = this.gameObject.transform.position;
        Vector2 itemPos = item.transform.position;
        return Vector2.Distance(thisPos, itemPos);
    }
    // ==============================================扫描函数========================================================
    void Scan()
    {
        foreach(Transform itemt in GameManager.itemList.GetComponentInChildren<Transform>())
        {
            GameObject item = itemt.gameObject;
            if(item.tag == "Weapon")
            {
                WeaponInfoGenerator generator = item.GetComponent<WeaponInfoGenerator>();
                if (Distance(item) <= scanRange)
                {
                    generator.ShowInfo();
                }
                else
                {
                    generator.HideInfo();
                }
            }
        }
    }

    void Update()
    {
        if(this.tag == "Player")
        {
            Scan();
        }
    }
}
