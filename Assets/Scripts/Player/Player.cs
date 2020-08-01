using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

//注：本脚本无法自动攻击不属于任何房间的敌人。

public class Player : MonoBehaviour, ICharacter
{
    //初始设定
    public float InitialHP, InitialMP, InitialMaxHP, InitialMaxMP, deathTime;
    [Range(0f, 1f)]
    public float fireAgility;
    [Range(0f, 1f)]
    public float moveAgility;
    [Range(0.5f, 20f)]
    public float weaponPickUpRange;
    [Range(0.5f, 20f)]
    public float itemPickUpRange;

    //血量
    private float hp, maxHP;
    public float MaxHP
    {
        get { return maxHP; }
        set
        {
            if (value <= 0) maxHP = 1; // 最大血量不可小于1
            else maxHP = value;
        }
    }
    public float HP
    {
        get { return hp; }
        set
        {
            if (value < 0) hp = 0;
            else if (value > MaxHP) hp = MaxHP;
            else hp = value;
        }
    }
    //能量
    private float mp, maxMP;
    public float MaxMP
    {
        get { return maxMP; }
        set
        {
            if (value <= 0) maxMP = 1; // 最大能量不可小于1
            else maxMP = value;
        }
    }
    public float MP
    {
        get { return mp; }
        set
        {
            if (value < 0) mp = 0;
            else if (value > MaxMP) mp = MaxMP;
            else mp = value;
        }
    }
    //金币
    private int coins;
    public int Coins
    {
        get { return coins; }
        set
        {
            if (value < 0) coins = 0;
            else coins = value;
        }
    }

    //潜行、行走速度、奔跑速度、受伤间隔
    public float sneak, walk, run, injureTime, rollTime, rollCoolingTime;
    [Header("AI数值")]
    [Tooltip("近战攻击距离")]
    public float meleeAttackDistance;
    [Tooltip("近战倾向")]
    [Range(0, 1f)]
    public float MelleTendency;


    //实际速度、伤害倒计时
    [HideInInspector]
    public float speed, injureTimer, motionTimer, deathTimer;
    [HideInInspector]
    public bool isAlive;
    [HideInInspector]
    public int lastFire;

    //参数
    private Vector3 direction;
    //我的物品
    private Arsenal arsenal;
    //场上物品
    private Transform itemList;
    //捡拾范围
    private CircleCollider2D pickArea;
    //渲染器
    private SpriteRenderer sr;
    //目标
    private GameObject target;
    private GameObject player;
    //状态
    [HideInInspector]
    public int status;

    void Start()
    {
        print(speed);
        //预设参数
        isAlive = true;
        deathTimer = deathTime;
        MaxHP = InitialMaxHP;
        MaxMP = InitialMaxMP;
        HP = InitialHP;
        MP = InitialMP;
        if (MelleTendency == 0) MelleTendency = 0.8f;
        if (fireAgility == 0) fireAgility = 1f;
        status = 0;
        //寻找：场景物品
        itemList = GameObject.Find("ItemList").transform;
        //寻找：武器控制
        arsenal = GetComponentInChildren<Arsenal>();
        //寻找：捡拾控制
        pickArea = GetComponentInChildren<CircleCollider2D>();
        pickArea.radius = itemPickUpRange;
        //寻找：渲染器
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        //倒计时
        if (motionTimer > -rollCoolingTime) motionTimer -= Time.deltaTime;

        //死亡判定
        if (HP <= 0)
        {
            Death();
        }
        else
        {
            //玩家
            if (CompareTag("Player"))
            {
                //特殊动作
                if (motionTimer <= 0)
                {
                    if (Input.GetKeyDown(KeyCode.Space) && motionTimer <= -rollCoolingTime)
                    {
                        status = 2;
                        motionTimer = rollTime;
                        speed = run;
                        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                        //GetComponent<CapsuleCollider2D>().enabled = false;
                    }
                    if (motionTimer <= 0)
                    {
                        status = 0;
                    }
                }
                else if (status == 2)
                {
                    transform.Translate(direction * speed * Time.deltaTime);
                    //if (motionTimer - 2 * Time.deltaTime <= 0)
                    //{
                    //    //GetComponent<CapsuleCollider2D>().enabled = true;
                    //}
                }

                //正常动作
                if (status == 0)
                {
                    //移动控制
                    speed = walk;
                    direction = GetDirection();
                    if (direction.x < 0) sr.flipX = true;
                    else if (direction.x > 0) sr.flipX = false;
                    transform.Translate(direction * speed * Time.deltaTime);

                    //激发武器
                    if (arsenal.CurrentWeapon() != null && (arsenal.CurrentWeapon().GetInfo("剩余冷却时间") <= 0 || !arsenal.CurrentWeapon().IsMelee()))
                    {
                        if (lastFire > 0)
                        {
                            arsenal.SwitchWeapon(lastFire);
                            lastFire = -1;
                        }

                        if (Input.GetButton("Fire1") && gameObject.layer == LayerMask.NameToLayer("Player"))
                        {
                            float _cost = arsenal.Attack();
                            if (_cost > 0)
                            {
                                MP -= _cost;
                            }
                            lastFire = -1;
                        }
                        else if (Input.GetButton("Fire2") && gameObject.layer == LayerMask.NameToLayer("Player"))
                        {
                            float _cost = arsenal.MeleeAttack();
                            if (_cost > 0)
                            {
                                MP -= _cost;
                            }
                            lastFire = arsenal.tempIndex;
                        }
                    }

                    //丢弃物品
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        arsenal.ThrowAway();
                    }
                    //捡起物品
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        arsenal.PickUp(GetClosestWeapon());
                    }
                    //切换武器
                    if (Input.GetAxis("Mouse ScrollWheel") != 0)
                    {
                        arsenal.SwitchWeapon();
                    }
                }

            }
            //随从
            else if (CompareTag("Retinue"))
            {
                if (status != 0)
                {
                    if (direction.x < 0) sr.flipX = true;
                    else if (direction.x > 0) sr.flipX = false;
                    transform.Translate(direction * speed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.I))
                {
                    print("转移物品到当前角色");
                    TransforAllItemsToPlayer();
                }
                bool hasEnemy;
                GameObject closestEnemy = WeaponBase.FindClosest(transform, WeaponBase.ENEMY);
                if (closestEnemy != null)
                {
                    hasEnemy = true;
                    target = closestEnemy;
                }
                else
                {
                    hasEnemy = false;
                    target = GameObject.FindGameObjectWithTag("Player");
                    if (target == null)
                    {
                        GameObject.Find("GameManager").GetComponent<GameManager>().ChangeCharacter();
                        target = GameObject.FindGameObjectWithTag("Player");
                    }
                }

                if (target == null) target = gameObject;
                //动作准备
                if ((target.transform.position - transform.position).sqrMagnitude > 25)
                {
                    status = -1;
                    if (target.CompareTag("Enemy") && arsenal.CurrentWeapon() != null && arsenal.CurrentWeapon().IsMelee()) speed = walk;
                    else speed = target.CompareTag("Player") ? walk : sneak;
                    //靠近目标
                    direction = (target.transform.position - transform.position).normalized;
                }
                else if (motionTimer <= 0 && (target.transform.position - transform.position).sqrMagnitude < 3)
                {
                    status = -1;
                    speed = target.CompareTag("Player") ? walk : sneak;
                    //远离目标
                    direction = -(target.transform.position - transform.position).normalized;
                    if (direction == Vector3.zero) direction = Random.insideUnitCircle.normalized;
                }
                else if (motionTimer <= 0)
                {
                    bool willMove = Random.Range(0f, 1f) < moveAgility;
                    if (willMove)
                    {
                        motionTimer = Random.Range(0.2f, 0.8f);
                        status = -1;
                        direction = Random.insideUnitCircle;
                    }
                    else
                    {
                        motionTimer = Random.Range(0.7f, 1f);
                        status = 0;
                    }
                }

                //开火
                if (hasEnemy == true)
                {
                    if (arsenal.CurrentWeapon() != null && arsenal.CurrentWeapon().IsMelee())
                    {
                        if ((target.transform.position - transform.position).sqrMagnitude < Mathf.Sqrt(meleeAttackDistance))
                        {
                            if (Random.Range(0f, 1f) < fireAgility) Attack();
                        }
                    }
                    else
                    {
                        if (Random.Range(0f, 1f) < fireAgility) Attack();
                    }
                }
            }

            //恢复颜色
            if (injureTimer > 0) injureTimer -= Time.deltaTime;
            else if (GetComponent<SpriteRenderer>().color != Color.white) GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private WeaponBase GetClosestWeapon()
    {
        List<WeaponBase> weaponBases = new List<WeaponBase>();
        foreach (Transform t in itemList)
        {
            if (t.CompareTag("Weapon"))
            {
                weaponBases.Add(t.GetComponent<WeaponBase>());
            }
        }
        WeaponBase weapon;
        if (weaponBases.Count == 0)
        {
            return null;
        }
        else
        {
            weapon = weaponBases[0];
        }
        foreach (WeaponBase w in weaponBases)
        {
            if ((w.transform.position - transform.position).sqrMagnitude < (weapon.transform.position - transform.position).sqrMagnitude)
            {
                weapon = w;
            }
        }
        if ((weapon.transform.position - transform.position).sqrMagnitude > Mathf.Sqrt(weaponPickUpRange))
        {
            print("武器距离角色太远");
            return null;
        }
        return weapon;
    }

    private void LateUpdate()
    {
        if (CompareTag("Retinue"))
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (transform == null || player == null) return;
            if ((player.transform.position - transform.position).sqrMagnitude > 625)
            {
                TransforRetinueToPlayer();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    void OnTriggerStay2D(Collider2D other)
    {
        // 随从与玩家
        if (other.gameObject.CompareTag("Item"))
        {
            if ((other.transform.position - transform.position).sqrMagnitude < 1)
            {
                ItemBase item = other.GetComponent<ItemBase>();
                item.Activate(this);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Indestructible") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Retinue") || collision.gameObject.CompareTag("Player"))
        {
            motionTimer = 0;
        }
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
            GetComponent<SpriteRenderer>().sortingLayerName = "Item";
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
    ///  工具函数
    /// </summary>
    /// 


    private Vector2 GetDirection()
    {
        if (CompareTag("Player"))
        {
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                return (Vector2.right * Input.GetAxisRaw("Horizontal") +
                           Vector2.up * Input.GetAxisRaw("Vertical") + TouchListener.instance.GetMovementRockerPos()).normalized;
            }
            else
            {
                return TouchListener.instance.GetMovementRockerPos().normalized;
            }
        }
        else
        {
            return Vector2.zero;
        }
    }

    public void Attack()
    {
        arsenal.Attack();
    }

    public void BeAttacked(float damage)
    {
        if (injureTimer > 0) return;
        HP -= damage;
        GetComponent<SpriteRenderer>().color = Color.red;
        injureTimer = injureTime;
    }

    public void BeAttacked(float damage, Vector3 impactForce)
    {
        if (injureTimer > 0) return;
        HP -= damage;
        GetComponent<SpriteRenderer>().color = Color.red;
        injureTimer = injureTime;
        GetComponent<Rigidbody2D>().AddForce(impactForce, ForceMode2D.Impulse);
    }

    public float GetInfo(string type)
    {
        if (type == "HP")
        {
            return HP;
        }
        else if (type == "MaxHP")
        {
            return MaxHP;
        }
        else if (type == "MaxMP")
        {
            return MaxMP;
        }
        else if (type == "MP")
        {
            return MP;
        }
        else if (type == "Coins")
        {
            return Coins;
        }
        else
        {
            Debug.LogError("没有该数据");
            return 0;
        }
    }

    void TransforAllItemsToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        foreach (ItemBase item in GetComponentsInChildren<ItemBase>(true))
        {
            item.GetTransform().transform.SetParent(player.transform.Find("Items"));
        }
    }

    public void TransforAllRetinuesToPlayer()
    {
        GameObject[] retinues = GameObject.FindGameObjectsWithTag("Retinue");
        foreach (GameObject retinue in retinues)
        {
            retinue.transform.position = transform.position;
        }
    }

    public void TransforRetinueToPlayer()
    {
        if (CompareTag("Retinue")) transform.position = player.transform.position;
    }

    public void DropDown(Transform item)
    {
        item.position = transform.position;
        item.gameObject.SetActive(true);
        item.rotation = Quaternion.Euler(Vector3.zero);
        item.SetParent(GameObject.Find("ItemList").transform);
        item.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle, ForceMode2D.Impulse);
    }
}