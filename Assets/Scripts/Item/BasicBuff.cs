using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBuff : ItemBase
{
    [Header("加减")]
    public int addCoin;
    public float addHP;
    public float addMP;
    public float addMaxHP;
    public float addMaxMP;
    [Header("乘除")]
    public int Coin;
    public float HP;
    public float MP;
    public float MaxHP;
    public float MaxMP;

    public override void Activate(Player character)
    {
        character.Coins += addCoin;
        character.MP += addMP;
        character.HP += addHP;
        character.MaxHP += addMaxHP;
        character.MaxMP += addMaxMP;

        character.Coins *= 1+Coin;
        character.MP *= 1+MP;
        character.HP *= 1+HP;
        character.MaxHP *= 1+MaxHP;
        character.MaxMP *= 1+MaxMP;

        Destroy(gameObject);
    }
}
