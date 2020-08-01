using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int MaxCharacterNum;
    [HideInInspector]
    static public Transform itemList;
    [HideInInspector]
    static public Transform bulletList;
    [HideInInspector]
    static public Transform characterList;
    [HideInInspector]
    static public Transform roomList;
    [HideInInspector]
    static public Transform enteranceWallList;

    private int index;
    private GameObject[] characters;
    void Start()
    {
        print("WASD上下左右移动；空格翻滚；滚轮切换武器；Q丢弃当前武器；E捡起当前武器；反键手动瞄准；Tab键切换角色。");
        CorrectPlayerNumber();

        itemList = GameObject.Find("ItemList").transform;
        bulletList = GameObject.Find("BulletList").transform;
        characterList = GameObject.Find("CharacterList").transform;
        roomList = GameObject.Find("RoomList").transform;
        enteranceWallList = GameObject.Find("EnteranceWallList").transform;
    }

    private void Update()
    {
        int count = 0;
        for (int i = 0; i < MaxCharacterNum; i++)
        {
            if (characters[i] == null) count++;
            else if (!characters[i].GetComponent<Player>().isAlive) count++;
        }

        if (count == MaxCharacterNum)
        {
            GameObject.Find("EndUIManager").GetComponent<EndUI>().OnGameEnd();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ChangeCharacter();
            }
        }

        if (characters[index] == null)
        {
            ChangeCharacter();
        }
        
    }

    private void LateUpdate()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length>1)
        {
            print("Player数量错误");
            CorrectPlayerNumber();
        }
    }


    public static void ChangeLayer(Transform trans, string targetLayer)
    {
        //遍历更改所有子物体Layer
        
        if (LayerMask.NameToLayer(targetLayer) == -1)
        {
            Debug.Log("Layer中不存在,请手动添加LayerName");
            return;
        }
        trans.gameObject.layer = LayerMask.NameToLayer(targetLayer);
        foreach (Transform child in trans)
        {
            ChangeLayer(child, targetLayer);
        }
    }


    public void ChangeCharacter()
    {
        if (characters[index] != null)
        {
            characters[index].tag = "Retinue";
            ChangeLayer(characters[index].transform, "Retinue");
        }
        while (true)
        {
            if (++index >= MaxCharacterNum) index = 0;
            if (characters[index] != null && characters[index].GetComponent<Player>().isAlive)
            {
                characters[index].tag = "Player";
                ChangeLayer(characters[index].transform, "Player");
                break;
            }
        }
    }

    public void ApplicationQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void CorrectPlayerNumber()
    {
        if (MaxCharacterNum <= 0) MaxCharacterNum = 1;
        GameObject[] retinues = GameObject.FindGameObjectsWithTag("Retinue");
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        if ((retinues.Length + player.Length) > MaxCharacterNum) Debug.LogError("角色超出上限");
        characters = new GameObject[MaxCharacterNum];
        player.CopyTo(characters, 0);
        retinues.CopyTo(characters, player.Length);
        for (int i = 0; i < MaxCharacterNum; i++) ChangeCharacter();
    }
}
