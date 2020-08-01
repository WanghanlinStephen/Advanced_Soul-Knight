using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickArea : MonoBehaviour
{
    GameObject character;
    void Start()
    {
        character = GetComponentInParent<Player>().gameObject;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            ItemBase item = other.GetComponent<ItemBase>();
            item.Fly2(character);
        }
    }
}
