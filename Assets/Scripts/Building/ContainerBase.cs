using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerBase : MonoBehaviour, IStructure
{
    [Header("花费（减去的数值）")]
    public int subCoin;
    public float subHP;
    public float subMP;
    public float subMaxHP;
    public float subMaxMP;
    [Header("花费（消耗的比例）")]
    [Range(0,1f)]
    public int Coin;
    [Range(0, 1f)]
    public float HP;
    [Range(0, 1f)]
    public float MP;
    [Range(0, 1f)]
    public float MaxHP;
    [Range(0, 1f)]
    public float MaxMP;
    [Header("是否展示物品")]
    public bool showItems;
    [Header("物品展示时间")]
    public float timeInterval;

    private bool empty;
    private List<IItem> items;

    void Start()
    {
        empty = false;
        items = new List<IItem>();
        foreach(Transform t in GetComponentsInChildren<Transform>())
        {
            IItem item = t.GetComponent<IItem>();
            if (item != null) 
            {
                items.Add(item);
                if (!showItems)
                {
                    t.gameObject.SetActive(false);
                    t.localPosition = Vector3.zero;
                }
                WeaponBase weapon = t.GetComponent<WeaponBase>();
                if (weapon != null)
                {
                    weapon.tag = "Item";
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!empty && Input.GetKey(KeyCode.E) && other.gameObject.CompareTag("Player"))
        {
            Player character= other.GetComponent<Player>();
            empty = Activate(character);
        }
    }

    bool Activate(Player character)
    {
        if (character.Coins >= subCoin &&
            character.HP > subHP &&
            character.MaxHP > subMaxHP &&
            character.MP >= subMP &&
            character.MaxMP >= subMaxMP)
        {
            character.Coins -= subCoin;
            character.MP -= subMP;
            character.HP -= subHP;
            character.MaxHP -= subMaxHP;
            character.MaxMP -= subMaxMP;
            character.Coins *= 1 - Coin;
            character.MP *= 1 - MP;
            character.HP *= 1 - HP;
            character.MaxHP *= 1 - MaxHP;
            character.MaxMP *= 1 - MaxMP;

            foreach (IItem item in items)
            {
                item.Drop(timeInterval);
            }
            GetComponent<SpriteRenderer>().color = Color.gray;
            return true;
        }
        else return false;
    }
}
