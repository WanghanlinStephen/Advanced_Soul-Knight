using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    [HideInInspector]
    public MapSize mapSize;
    private MapManager mapManager;


    private void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GetComponentInParent<EnemyArea>().stay = true;
            if (!GetComponentInParent<EnemyArea>().come) 
            { 
                mapManager.CloseBoxCollider();
                collision.GetComponent<Player>().TransforAllRetinuesToPlayer();
            }
            GetComponentInParent<EnemyArea>().come = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) GetComponentInParent<EnemyArea>().stay = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponentInParent<EnemyArea>().stay = false;
        }
    }
}
