using UnityEngine;

public class Enemy : MonoBehaviour, ICharacter
{
    //初始化信息
    public float hp;
    //血量
    public float HP { get; set; }
    //近身率、激发率、行走、奔跑、无敌时间
    public float closeToTargetRate, fireRate, walk, run, injureTime, deathTime;
    //是否可被击退、是否活着
    public bool repelable, isAlive;

    //行动冷却时间、受伤冷却时间、速度
    [HideInInspector]
    public float motionTimer, injureTimer, speed;
    //携带的物品
    [HideInInspector]
    public Transform items;

    //场上的物品
    private Transform itemList;
    private GameObject target;
    private IWeapon weapon;
    private Vector3 direction;
    private bool isMelee;
    private float deathTimer;


    void Start()
    {
        isAlive = true;
        HP = hp;
        deathTimer = deathTime;
        speed = walk;
        //获取基本信息
        target = GameObject.FindGameObjectWithTag("Player");
        weapon = GetComponentInChildren<IWeapon>();
        itemList = GameObject.Find("ItemList").transform;
        if (weapon.IsMelee())
        {
            isMelee = true;
        }
        GameManager.ChangeLayer(transform, "Enemy");
    }


    void Update()
    {
        //死亡判定
        if (HP <= 0)
        {
            Death();
        }
        else
        {
            //动作准备
            if (motionTimer <= 0)
            {
                //随机动作时间
                motionTimer = Random.Range(0.8f, 2f);
                //接近玩家
                if (target != null && (Random.Range(0f, 1f) < closeToTargetRate))
                {
                    direction = (target.transform.position - transform.position).normalized;
                    if (isMelee)
                    {
                        speed = run;
                    }
                    else
                    {
                        speed = walk;
                    }
                }
                //或者游荡
                else
                {
                    direction = (new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0)).normalized;
                    speed = run;
                }
            }
            else
            {
                //非近战单位随机开火(非近战、有武器、随机成立)
                target = WeaponBase.FindClosest(transform, WeaponBase.FRIEND);
                float temp = Random.Range(0f, 1f);
                if (target != null)
                {
                    if (!isMelee && temp < fireRate)
                    {
                        weapon.Attack();
                    }
                    else if (isMelee && temp < fireRate && (target.transform.position - transform.position).sqrMagnitude < 2f)
                    {
                        weapon.Attack();
                    }
                }
            }
            //倒计时
            motionTimer -= Time.deltaTime;

            //移动
            transform.Translate(direction * speed * Time.deltaTime);
            //恢复颜色
            if (injureTimer >= 0) injureTimer -= Time.deltaTime;
            else if (GetComponent<SpriteRenderer>().color != Color.white) GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Indestructible")) direction = (new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0)).normalized;
    }

    /// <summary>
    /// 动作函数：逐帧调用的函数
    /// </summary>
    private void Death()
    {
        if (isAlive)
        {
            /* 死亡机制应配合死亡动画，暂时以拉灰、放倒代替 */
            isAlive = false;
            GetComponent<CapsuleCollider2D>().enabled = false;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            direction = Vector3.zero;
            speed = 0;
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
        else if (deathTimer > 0)
        {
            deathTimer -= Time.deltaTime;
        }
        else
        {
            //死亡掉落
            foreach (ItemBase item in GetComponentsInChildren<ItemBase>(true))
            {
                DropDown(item.GetTransform());
            }
            //删除对象
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 工具函数：任意使用的函数
    /// </summary>
    public void InitializeRole()
    {
        HP = hp;
    }


    public float GetInfo(string type)
    {
        if (type == "HP") return HP;
        else if (type == "MaxHP")
        {
            return hp;
        }
        else
        {
            Debug.LogError("没有该数据");
            return 0;
        }
    }

    public void Attack()
    {
        GetComponentInChildren<IWeapon>().Attack();
    }

    public void BeAttacked(float damage, Vector3 impactForce)
    {
        if (injureTimer > 0 || HP <= 0) return;
        HP -= damage;
        GetComponent<SpriteRenderer>().color = Color.red;
        injureTimer = injureTime;
        GetComponent<Rigidbody2D>().AddForce(impactForce, ForceMode2D.Impulse);
    }

    public void DropDown(Transform item)
    {
        item.position = transform.position;
        item.gameObject.SetActive(true);
        item.rotation = Quaternion.Euler(Vector3.zero);
        item.SetParent(itemList);
        item.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle, ForceMode2D.Impulse);
    }
    public void PickUp(Transform item)
    {
        item.SetParent(items);
        item.gameObject.SetActive(false);
    }
}
