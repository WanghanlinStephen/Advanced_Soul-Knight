using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase: MonoBehaviour, IItem
{
    [HideInInspector]
    public float coolingTimer;
    [HideInInspector]
    public Rigidbody2D rb;
    private float speed;
    private bool flyToplayer, flyable;
    private GameObject character;

    void Start()
    {
        speed = 10f;
        flyable = true;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (flyable && flyToplayer && this != null && character != null)
        {
            rb.velocity = (character.transform.position - transform.position).normalized * speed;
        }
        if (coolingTimer > 0) coolingTimer -= Time.deltaTime;
        else
        {
            GetComponent<Collider2D>().enabled = true;
        }
    }

    public void Fly2(GameObject character)
    {
        if (coolingTimer > 0) return;
        flyToplayer = true;
        this.character = character;
    }
    public Transform GetTransform()
    {
        return transform;
    }
    public void PickBy(Transform t)
    {
        transform.SetParent(t);
        gameObject.SetActive(false);
        flyable = false;
    }
    public void Drop()
    {
        gameObject.SetActive(true);
        flyable = true;
        transform.SetParent(GameManager.itemList);
    }
    public void Drop(float coolingTime)
    {
        gameObject.SetActive(true);
        flyable = true;
        transform.SetParent(GameManager.itemList);
        rb.AddForce(Random.insideUnitCircle*5, ForceMode2D.Impulse);
        coolingTimer = coolingTime;
        GetComponent<Collider2D>().enabled = false;
    }

    public abstract void Activate(Player character);
}
