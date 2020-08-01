using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour, IWeapon
{
    [HideInInspector]
    [Tooltip("攻击力")]
    public float damage;
    [HideInInspector]
    [Tooltip("冷却时间")]
    public float coolingTime;
    [HideInInspector]
    [Tooltip("击退")]
    public float impactForce;
    [Tooltip("挥刀音效")]
    public AudioClip sound;
    [Range(0f, 1f)]
    public float volume;
    [Tooltip("突刺时间")]
    public float stabTime;
    [Tooltip("收刺时间")]
    public float backTime;
    [Tooltip("突刺速度")]
    public float stabSpeed;
    [Tooltip("费用")]
    [HideInInspector]
    public float cost;

    public bool IsMelee() { return true; }

    private EdgeCollider2D edgeCollider2D;
    private float coolingTimer, motionTimer, backSpeed ;
    private bool stab;
    private int step;
    private Vector3 originalposition;

    void Start()
    {
        originalposition = transform.localPosition;
        edgeCollider2D = GetComponent<EdgeCollider2D>();
        edgeCollider2D.isTrigger = true;
        edgeCollider2D.enabled = false;
        coolingTime = stabTime + backTime > coolingTime ? stabTime + backTime : coolingTime;
        backSpeed = stabSpeed * stabTime / backTime;
    }

    void Update()
    {
        if (stab) Stab();
        if (coolingTimer > 0) coolingTimer -= Time.deltaTime;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (this.gameObject.layer == LayerMask.NameToLayer("Player") || this.gameObject.layer == LayerMask.NameToLayer("Retinue"))
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<ICharacter>().BeAttacked(damage, (collision.transform.position - transform.position).normalized * impactForce);
            }
        }
        else if(this.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (collision.CompareTag("Player") || collision.CompareTag("Retinue"))
            {
                collision.GetComponent<ICharacter>().BeAttacked(damage, (collision.transform.position - transform.position).normalized * impactForce);
            }
        }
    }
    /// <summary>
    /// 工具函数
    /// </summary>
    public float Attack()
    {
        if (coolingTimer <= 0 && step == 0)
        {
            coolingTimer = coolingTime;
            stab = true;
            return cost;
        }
        else
        {
            return -1f;
        }
    }
    /// <summary>
    /// 动作函数
    /// </summary>
    //刺
    public void Stab()
    {
        //准备 
        if (step == 0)
        {
            transform.localPosition = originalposition;
            motionTimer = stabTime;
            step = 1;
            edgeCollider2D.enabled = true;
            AudioSource.PlayClipAtPoint(sound, Vector3.zero, volume);
        }
        //刺
        else if (step == 1)
        {

            transform.localPosition += new Vector3(-stabSpeed * Time.deltaTime, 0, 0);
            motionTimer -= Time.deltaTime;
            if (motionTimer <= 0)
            {
                motionTimer = backTime;
                step = 2;
            }
        }
        //收
        else if(step==2)
        {
            transform.localPosition += new Vector3(backSpeed * Time.deltaTime, 0, 0);
            motionTimer -= Time.deltaTime;
            if (motionTimer <= 0)
            {
                step = 3;
                edgeCollider2D.enabled = false;
            }
        }
        //结束
        else if (step == 3)
        {
            transform.localPosition = originalposition;
            step = 0;
            stab = false;
        }
    }
    public float GetInfo(string type)
    {
        if (type == "伤害")
        {
            return damage;
        }
        else if (type == "冷却")
        {
            return coolingTime;
        }
        else if (type == "击退")
        {
            return impactForce;
        }
        else if (type == "费用")
        {
            return cost;
        }
        else if (type == "剩余冷却时间")
        {
            return coolingTimer;
        }
        else
        {
            throw new UnityException("信息不存在。");
        }
    }

    public void SetInfo(string type, float value)
    {
        if (type == "伤害")
        {
            damage = value;
        }
        else if (type == "冷却")
        {
            coolingTime = value;
        }
        else if (type == "击退")
        {
            impactForce = value;
        }
        else if (type == "费用")
        {
            cost = value;
        }
        else
        {
            throw new UnityException("信息不存在。");
        }
    }
}
