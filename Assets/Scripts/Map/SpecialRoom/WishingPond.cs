using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishingPond : MonoBehaviour,ISpecialRoom
{
    // Start is called before the first frame update
    public int expenditure=10;
    public int numberOfwishes=0;
    public int numberOffloor=0;
    public float money=0;
    public WishingPond(int expenditure){
        this.expenditure=expenditure;
    }
    public GameObject Button;
    
    void Start()
    {
    }
    public void Update()
    {
          
        if(Input.GetKeyDown(KeyCode.Y)){

            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            Player here = player[0].gameObject.GetComponent<Player>();
            money = here.GetInfo("Coins");
            bool result = MakeWish();
            if(result == true){
                money-=expenditure;
                print("Successful");
            }else{
                print("Unsuccessful");
            }
        }
         

    }
    public bool MakeWish()
    {
        if(money>expenditure /* *numberOfwishes+10*numberOffloor */){
            numberOfwishes++;
            return true;
        }
        return false;
    }
}
