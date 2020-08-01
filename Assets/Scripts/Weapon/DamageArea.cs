using System.Collections;
using System.Collections.Generic;
using UnityEngine;
               
public class DamageArea : MonoBehaviour
{
    //伤害半径、生命期、伤害值、伤害间距
    public float radius, lifetime, damage, damageTimeGap;

    //计时器
    private float timer;

    //是否对友方、敌方造成伤害、是否为持续伤害
    public bool friendDmg, enemyDmg, noInterval;
   
    //伤害范围
    public CircleCollider2D damageArea;
    public Animator anim;
    public SpriteRenderer sr;

    void Start()
    {  
        //初始化
        damageArea = gameObject.AddComponent<CircleCollider2D>() as CircleCollider2D;
        damageArea.radius = this.radius;
        damageArea.isTrigger = true;
        timer = damageTimeGap;
        if (damageTimeGap == 0) damageTimeGap = 1f;
    }

    void Update()
    {
        if (lifetime <= 0) Destroy(gameObject);
        else lifetime -= Time.deltaTime;
        if (timer > 0) timer -= Time.deltaTime;
    }

    //进入
    void OnTriggerEnter2D(Collider2D col)
    {
        //瞬间伤害：直接扣血
        if (!noInterval)
        {
            if ((col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Retinue")) && friendDmg) col.gameObject.GetComponent<Player>().HP -= damage;
            else if (col.gameObject.CompareTag("Enemy") && enemyDmg) col.gameObject.GetComponent<Enemy>().HP -= damage;
        }
    }

    //停留
    void OnTriggerStay2D(Collider2D col)
    {
        //瞬间伤害：间断扣血
        if (!noInterval)
        {
            if ((col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Retinue")) && friendDmg)
            {
                if (damageTimeGap <= 0)
                {
                    col.gameObject.GetComponent<Player>().HP -= damage;
                    timer = damageTimeGap;
                }
            }
            else if (col.gameObject.CompareTag("Enemy") && enemyDmg)
            {
                if (damageTimeGap <= 0)
                {
                    col.gameObject.GetComponent<Enemy>().HP -= damage;
                    timer = damageTimeGap;
                }
            }
        }
        //持续伤害：持续扣血
        else
        {
            if ((col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Retinue")) && friendDmg) if (damageTimeGap <= 0) col.gameObject.GetComponent<Player>().HP -= damage * Time.deltaTime;
            else if (col.gameObject.CompareTag("Enemy") && enemyDmg) if (damageTimeGap <= 0) col.gameObject.GetComponent<Enemy>().HP -= damage * Time.deltaTime;
        }
    }
}