using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    //怪物列表、物品列表
    public List<GameObject> enemies;
    public GameObject[] itemList;
    //长、宽
    public int width, height;
    //剩余波数
    private float waveCount;
    //玩家是否来过房间、正在房间里、已过关
    [HideInInspector]
    public bool come, stay, pass;

    void Start()
    {
        pass = false;
        stay = false;
        waveCount = Random.Range(1, 5);
    }

    void Update()
    {
        if (stay&&!pass)
        {
            if (GetComponentsInChildren<Enemy>().Length == 0)
            {
                if ( waveCount >= 0)
                {
                    AddEnemys(Random.Range(3,7));
                    waveCount--;
                }
                else
                {
                    GameObject.Find("MapManager").GetComponent<MapManager>().OpenBoxCollider();
                    pass = true;
                }
            }
        }
    }

    void AddEnemys(int num)
    {
        for (int i=0; i< num; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(3f, width - 3f), Random.Range(3f, height - 3f));
            int index = Random.Range(0, enemies.Count);
            GameObject enemy = Instantiate(enemies[index].gameObject, transform.position + randomPosition, Quaternion.identity);
            for(int n = 0; n < Random.Range(3, 5); n++)
            {
                int index2 = Random.Range(0, itemList.Length);
                GameObject item = GameObject.Instantiate(itemList[index2]);
                item.name = itemList[index2].name;
                enemy.GetComponent<Enemy>().PickUp(item.transform);
            }
            enemy.transform.SetParent(transform);
            enemy.SetActive(true);
        }
    }

}
