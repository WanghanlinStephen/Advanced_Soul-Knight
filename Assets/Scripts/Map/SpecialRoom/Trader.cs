using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : MonoBehaviour,ISpecialRoom
{
    // Start is called before the first frame update
    public int expenditure=10;
    public int numberOffloor=0;
    public float money=0;
    public Trader(int expenditure){
        this.expenditure=expenditure;
    }
    public GameObject Button;
    void Start()
    {
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)){
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            Player here = player[0].gameObject.GetComponent<Player>();
            money = here.GetInfo("Coins");
            bool result = MakeTrade();
            if(result == true){
                money-=expenditure;
                here.Coins=(int)money;
                print("Successful Purchased");
            }else{
                print("Money is insufficient");
            }
        }    
    }
    public bool MakeTrade()
    {
        if(money>expenditure /* *numberOfwishes+10*numberOffloor */){
            return true;
        }
        return false;
    }
}
