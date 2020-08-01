using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 玩家的武器管理组件：鼠标滚轮切换武器；鼠标正键攻击。
/// </summary>
public class Arsenal : MonoBehaviour
{
    public int MaxNumOfWeapon;

    [HideInInspector]
    public List<GameObject> weapons;
    private int index;
    [HideInInspector]
    public int tempIndex;
    private Transform itemList;

    void Start()
    {
        SpriteRenderer sr;
        weapons = new List<GameObject>();
        itemList = GameObject.FindGameObjectWithTag("ItemManager").transform;
        foreach (Transform t in transform)
        {
            //将该物体下所有Sprite Renderer的Sorting Layer改为Handheld Item。
            sr = t.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.sortingLayerName = "Handheld Item";
            //若物体为Weapon，则置于图层顶部。
            if (t.CompareTag("Weapon"))
            {
                weapons.Add(t.gameObject);
                t.gameObject.SetActive(false);
                sr.sortingOrder = 1;
            }
        }
    }

    public IWeapon CurrentWeapon()
    {
        if (weapons.Count == 0) return null;
        else 
        {
            return weapons[index].GetComponent<WeaponBase>().weapon; 
        }
    }

    public float MeleeAttack()
    {
        foreach (GameObject _weapon in weapons)
        {
            IWeapon weapon = _weapon.GetComponentInChildren<IWeapon>();
            if (weapon != null && weapon.IsMelee())
            {
                tempIndex = index;
                SwitchWeapon(weapons.IndexOf(_weapon));
                return Attack();
            }
        }
        return -1f;
    }

    public float Attack()
    {
        if (weapons.Count > 0)
        {
            if (weapons[index] == null) 
            { 
                print(index);
                return -1f; 
            }
            return weapons[index].GetComponentInChildren<IWeapon>().Attack();
        }
        else
        {
            return -1f;
        }
    }

    public void ThrowAway()
    {
        if (weapons.Count == 0) return;
        weapons[index].GetComponent<WeaponBase>().Drop();
        weapons.Remove(weapons[index]);
        if (index >= weapons.ToArray().Length) index = 0;
        SwitchWeapon();
    }

    public void PickUp(WeaponBase weapon)
    {
        if (weapon==null) return;
        else if (weapons.Count >= MaxNumOfWeapon)
        {
            ThrowAway();
        }
        weapons.Add(weapon.gameObject);
        weapon.PickBy(transform);
        SwitchWeapon(weapon);
    }

    public void SwitchWeapon()
    {
        if (weapons.Count == 0) return;
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            weapons[index].SetActive(false);
            index += 1;
            if (index >= weapons.ToArray().Length) index = 0;
            weapons[index].SetActive(true);
        }
        else
        {
            weapons[index].SetActive(false);
            index -= 1;
            if (index < 0) index = weapons.ToArray().Length - 1;
            weapons[index].SetActive(true);
        }
    }

    public void SwitchWeapon(int n)
    {
        if (0 >= weapons.Count) return;
        index = n;
        foreach(GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
        weapons[n].SetActive(true);
    }

    public void SwitchWeapon(WeaponBase weapon)
    {
        if (weapons.Count <=0 || weapon == null) return;
        foreach (GameObject _weapon in weapons)
        {
            if (_weapon.GetComponent<WeaponBase>()==weapon)
            {
                SwitchWeapon(weapons.IndexOf(_weapon));
                return;
            }
        }
        Debug.LogWarning("没有该武器");
    }
}
