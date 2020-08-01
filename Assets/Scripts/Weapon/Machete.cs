using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machete : MonoBehaviour, IWeapon
{
    [Tooltip("攻击力")]
    [HideInInspector]
    public float damage;
    [Tooltip("冷却时间")]
    [HideInInspector]
    public float coolingTime;
    [Tooltip("击退")]
    [HideInInspector]
    public float impactForce;
    [Tooltip("挥刀音效")]
    public AudioClip sound;
    [Range(0f, 1f)]
    public float volume;
    [Tooltip("举刀速度")]
    public float upSpeed;
    [Tooltip("举刀时间")]
    public float upTime;
    [Tooltip("落刀速度")]
    public float downSpeed;
    [Tooltip("落刀时间")]
    public float downTime;
    [Tooltip("费用")]
    [HideInInspector]
    public float cost;

    public bool IsMelee() { return true; }

    private EdgeCollider2D edgeCollider2D;
    private WeaponBase aiming;
    private float coolingTimer, motionTimer;
    private int step;
    private bool chop;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    void Start()
    {
        edgeCollider2D = GetComponent<EdgeCollider2D>();
        edgeCollider2D.enabled = false;
        aiming = GetComponentInParent<WeaponBase>();
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
        coolingTime = upTime + downTime > coolingTime ? upTime + downTime : coolingTime;
    }

    void Update()
    {
        if (coolingTime > 0)
        {
            coolingTimer -= Time.deltaTime;
        }

        if (chop)
        {
            Chop();
            if (motionTimer > 0)
            {
                motionTimer -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.gameObject.layer == LayerMask.NameToLayer("Player") || this.gameObject.layer == LayerMask.NameToLayer("Retinue"))
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<ICharacter>().BeAttacked(damage, (collision.transform.position-transform.position).normalized * impactForce);
            }
            else if(collision.CompareTag("Hostile Bullet"))
            {
                Destroy(collision.gameObject);
            }
        }
        else if (this.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (collision.CompareTag("Player") || collision.CompareTag("Retinue"))
            {
                collision.GetComponent<ICharacter>().BeAttacked(damage, (collision.transform.position - transform.position).normalized * impactForce);
            }
        }
    }

    public float Attack()
    {
        if (coolingTimer <= 0)
        {
            chop = true;
            coolingTimer = coolingTime;
            return cost;
        }
        else
        {
            return -1f;
        }
    }

    void Chop()
    {
        //准备
        if (step == 0)
        {
            motionTimer = upTime;
            step++;
        }
        //抬刀
        else if (step == 1)
        {
            transform.RotateAround(transform.parent.position, aiming.scale.x * Vector3.back, upSpeed *  Time.deltaTime);
            if (motionTimer <= 0)
            {
                edgeCollider2D.enabled = true;
                motionTimer = downTime;
                step++;
                AudioSource.PlayClipAtPoint(sound, Vector3.zero, volume);
            }
        }
        //挥刀
        else if (step == 2)
        {
            transform.RotateAround(transform.parent.position, aiming.scale.x * Vector3.forward, downSpeed * Time.deltaTime);
            if (motionTimer <= 0)
            {
                step++;
            }
        }
        //结束
        else if (step == 3)
        {
            transform.localRotation = originalRotation;
            transform.localPosition = originalPosition;
            edgeCollider2D.enabled = false;
            step = 0;
            chop = false;
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
            Debug.LogError("无法修改该数据");
        }
    }
}
