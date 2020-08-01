using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;//Position of the target
    public float speed = 5f;//Speed of the camera

    // LateUpdate is called once per frame after Update() has been called
    void LateUpdate(){
        target = GameObject.FindGameObjectWithTag("Player");
        if (target == null) return;
        //The posotion of the player
        Vector3 targetPosition= new Vector3(target.transform.position.x,target.transform.position.y,transform.position.z);
        //Move towards player with time delay
        if(transform.position!=target.transform.position){
            transform.position=Vector3.Lerp(transform.position,targetPosition,speed * Time.deltaTime);
        }
    }
}
