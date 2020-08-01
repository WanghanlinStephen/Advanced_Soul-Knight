using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, IWeapon
{
    //枪管、枪口、子弹
    [HideInInspector]
    public Transform Barrel,Muzzle,Bullet;
    //枪声
    public AudioClip sound;
    //冷却
    [HideInInspector]
    public float coolingTime;
    //费用
    [HideInInspector]
    public float cost;
    //伤害
    [HideInInspector]
    public float damage;
    //音量
    [Range(0f,1f)]
    public float volume;
    //精度（散布角度）
    [Range(0f, 180f)]
    public float accuracy;
    //每发弹片数
    public int shrapnelPerShot;
    public bool IsMelee() { return false; }

    [HideInInspector]
    public float d;
    private float coolingTimer;
    private GameObject BulletList;
    private Bullet bullet;
    private float impactForce;

    void Start()
    {
        BulletList = GameObject.Find("BulletList");
        if (sound == null) sound = AudioClip.Create("MySinusoid", 44100 * 2, 1, 44100, true);
        if (shrapnelPerShot <= 0) shrapnelPerShot = 1;

        Barrel = transform.Find("Barrel");
        Muzzle = transform.Find("Muzzle");
        Bullet = transform.Find("Bullet");
    }

    void Update()
    {
        if (coolingTimer > 0)coolingTimer -= Time.deltaTime;
    }

    public float Attack()
    {
        if (coolingTimer <= 0)
        {
            for (int i = 0; i < shrapnelPerShot; i++)
            {
                GameObject go = GameObject.Instantiate(Bullet.gameObject, Muzzle, true);
                go.SetActive(true);
                bullet = go.GetComponent<Bullet>();
                bullet.weapon = this;

                bullet.damage = damage;
                bullet.impactForce = impactForce;

                d = Muzzle.position.x < Barrel.position.x ? -1f : 1f;
                bullet.d = d;
                bullet.transform.Rotate(new Vector3(0, 0, Random.Range(-accuracy, accuracy)));
                go.transform.SetParent(BulletList.transform);
                if (bullet.trace) bullet.target = GetComponentInParent<WeaponBase>().target;
                if (this.gameObject.layer == LayerMask.NameToLayer("Player")) bullet.tag = "Friendly Bullet";
                else if (this.gameObject.layer == LayerMask.NameToLayer("Retinue")) bullet.tag = "Friendly Bullet";
                else if (this.gameObject.layer == LayerMask.NameToLayer("Enemy")) bullet.tag = "Hostile Bullet";
            }
            AudioSource.PlayClipAtPoint(sound, Vector3.zero, volume);
            coolingTimer = coolingTime;
            return cost;
        }
        else
        {
            return -1f;
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
