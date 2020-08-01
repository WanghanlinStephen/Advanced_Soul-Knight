using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 所有武器的基对象的组件，用于搭载武器触发器、控制武器姿态等。
/// </summary>
public class WeaponBase : MonoBehaviour, IItem
{
    public const int ENEMY = -1;
    public const int FRIEND = 1;

    public float
        damage,
        coolingTime,
        impactForce,
        cost;

    [HideInInspector]
    public GameObject target;
    [HideInInspector]
    public Vector3 scale;
    [HideInInspector]
    public IWeapon weapon;

    private bool isUsing;
    private float fireangle, coolingTimer;
    private Vector3 targetPosition;
    private CircleCollider2D interArea;

    // Layer
    private int enemyLayer, playerLayer, retinueLayer;
    public int EnemyLayer { get => enemyLayer; set => enemyLayer = value; }
    public int PlayerLayer { get => playerLayer; set => playerLayer = value; }
    public int RetinueLayer { get => retinueLayer; set => retinueLayer = value; }
    public bool IsUsing 
    { 
        get => isUsing; 
        set 
        {
            isUsing = value;
            if (value)
            {
                interArea.enabled = false;
                GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Handheld Item";
            }
            else 
            {
                interArea.enabled = true;
                GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Item";
            }
        } 
    }

    void Start()
    {
        EnemyLayer = LayerMask.NameToLayer("Enemy");
        PlayerLayer = LayerMask.NameToLayer("Player");
        RetinueLayer = LayerMask.NameToLayer("Retinue");
        interArea = GetComponent<CircleCollider2D>();
        if (interArea == null)
        {
            gameObject.AddComponent<CircleCollider2D>();
            interArea = GetComponent<CircleCollider2D>();
            interArea.isTrigger = true;
        }
        weapon = GetComponentInChildren<IWeapon>();
        if (weapon == null) Debug.LogError("未找到武器组件");
        else
        {
            weapon.SetInfo("伤害", damage);
            weapon.SetInfo("冷却", coolingTime);
            weapon.SetInfo("击退", impactForce);
            weapon.SetInfo("费用", cost);
        }
        IsUsing = GetComponentInParent<ICharacter>() != null;
    }

    void Update()
    {
        if (coolingTimer > 0) coolingTimer -= Time.deltaTime;
        else
        {
            tag = "Weapon";
        }
        if (IsUsing)
        {
            if (GetComponentInParent<ICharacter>().GetInfo("HP") <= 0) Destroy(gameObject);
            //寻找目标
            else if (gameObject.layer == EnemyLayer)
            {
                target = FindClosest(transform, FRIEND);
            }
            else if (gameObject.layer == RetinueLayer)
            {
                target = FindClosest(transform, ENEMY);
            }
            else if (gameObject.layer == PlayerLayer)
            {
                target = FindClosest(transform, ENEMY);
            }
            else
            {
                print("对象层级错误！");
            }
            //瞄准目标
            targetPosition = target == null ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : target.transform.position;
            Vector2 targetDir = targetPosition - transform.position;
            if (gameObject.layer == RetinueLayer && false) fireangle = 0;//开启or禁用随从瞄准鼠标
            else fireangle = Vector2.Angle(targetDir, Vector3.up);
            scale = targetPosition.x > transform.position.x ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
            this.transform.eulerAngles = new Vector3(0, 0, (fireangle - 90) * scale.x);
            this.transform.localScale = scale;
        }
    }

    //返回玩家所在的房间内最近的敌人，或者最近的玩家。
    public static GameObject FindClosest(Transform self, int type)
    {
        if (self.GetComponentInParent<ICharacter>().GetInfo("HP") <= 0) return null;
        GameObject target = null;
        //返回最近的Player类对象
        if (type == FRIEND)
        {
            List<Player> characters = new List<Player>();
            foreach (Player character in GameObject.Find("CharacterList").GetComponentsInChildren<Player>())
            {
                if (character.HP >= 0) characters.Add(character);
            }

            if (characters.Count > 0)
            {
                target = characters[0].gameObject;
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i].HP >= 0 && (characters[i].transform.position - self.position).sqrMagnitude < (target.transform.position - self.position).sqrMagnitude)
                    {
                        target = characters[i].gameObject;
                    }
                }
            }
        }
        //返回最近的Enemy类对象
        else if (type == ENEMY)
        {
            List<Enemy> enemies = new List<Enemy>();
            EnemyArea[] enemyAreas = GameObject.Find("RoomList").GetComponentsInChildren<EnemyArea>();

            foreach (EnemyArea enemyArea in enemyAreas)
            {
                if (enemyArea.stay)
                {
                    foreach(Enemy enemy in enemyArea.GetComponentsInChildren<Enemy>())
                    {
                        if (enemy.HP >= 0) enemies.Add(enemy);
                    }
                }
            }

            if (enemies.Count > 0)
            {
                target = enemies[0].gameObject;
                for (int i = 0; i < enemies.Count; i++)
                {
                    if ((enemies[i].transform.position - self.position).sqrMagnitude < (target.transform.position - self.position).sqrMagnitude)
                    {
                        target = enemies[i].gameObject;
                    }
                }
            }
        }

        if (target != null && target.GetComponentInChildren<ICharacter>().GetInfo("HP") <= 0) target = null;

        return target;
    }

    public void Drop()
    {
        gameObject.SetActive(true);
        IsUsing = false;
        transform.SetParent(GameManager.itemList);
        transform.rotation = Quaternion.identity;
    }

    public void Drop(float coolingTime)
    {
        gameObject.SetActive(true);
        IsUsing = false;
        transform.SetParent(GameManager.itemList);
        transform.rotation = Quaternion.identity;
    }

    public void PickBy(Transform t)
    {
        transform.SetParent(t);
        IsUsing = true;
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }

}
