using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public IWeapon weapon;
    [HideInInspector]
    public GameObject target;
    //方向
    public Vector3 direction;
    //速度、周期、伤害、冲击力、方向
    public float speed, lifeTime, d;
    [HideInInspector]
    public float damage, impactForce;
    //命中声、爆炸声
    public AudioClip hitSound, explodeSound;
    //音量
    [Range(0f, 1f)]
    public float volume;
    //是否会爆炸、是否跟踪目标
    public bool detonable, trace;


    void Start()
    {
        direction = Vector3.right;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void Update()
    {
        if (trace && target != null)
        {
            //追踪阶段
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);

            transform.LookAt(target.transform,Vector3.right);
            transform.Rotate(new Vector3(0, -90, 0));

            transform.right = ((target.transform.position - transform.position)*d).normalized;
        }
        else transform.Translate(d * direction * speed * Time.deltaTime);

        //生命周期
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0) Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Indestructible"))
        {
            if (detonable)
            {
                if (explodeSound != null) AudioSource.PlayClipAtPoint(explodeSound, new Vector3(0, 0, 0), volume);
                GameObject damageArea = new GameObject();
                damageArea.transform.position = transform.position;
                damageArea.AddComponent<DamageArea>();
                damageArea.GetComponent<DamageArea>().radius = 3f;
                damageArea.GetComponent<DamageArea>().lifetime = 1f;
            }
            Destroy(gameObject);
        }
        //友方子弹
        else if (CompareTag("Friendly Bullet"))
        {
            if (collision.CompareTag("Enemy"))
            {
                //致伤
                Enemy enemy = collision.GetComponent<Enemy>();
                enemy.BeAttacked(damage, d * transform.right * impactForce);

                if (detonable)
                {
                    if (explodeSound != null) AudioSource.PlayClipAtPoint(explodeSound, new Vector3(0, 0, 0), volume);
                    GameObject damageArea = new GameObject();
                    damageArea.transform.position = transform.position;
                    damageArea.AddComponent<DamageArea>();
                    DamageArea d = damageArea.GetComponent<DamageArea>();
                    d.radius = 3f;
                    d.lifetime = 0.1f;
                    d.damage = damage;
                    d.enemyDmg = true;
                    d.noInterval = false;
                }
                Destroy(gameObject);
            }
        }
        //敌方子弹
        else if (CompareTag("Hostile Bullet"))
        {
            if (collision.CompareTag("Player") || collision.CompareTag("Retinue"))
            {
                if (collision.CompareTag("Player") && collision.GetComponent<Player>().status == 2) return;
                //致伤
                Player player = collision.GetComponent<Player>();
                player.BeAttacked(damage, d * transform.right * impactForce);

                if (detonable)
                {
                    if (explodeSound != null) AudioSource.PlayClipAtPoint(explodeSound, new Vector3(0, 0, 0), volume);
                    GameObject damageArea = new GameObject();
                    damageArea.transform.position = transform.position;
                    damageArea.AddComponent<DamageArea>();
                    DamageArea d = damageArea.GetComponent<DamageArea>();
                    d.radius = 3f;
                    d.lifetime = 0.1f;
                    d.damage = damage;
                    d.friendDmg = true;
                }
                Destroy(gameObject);
            }
        }
            
    }
}
