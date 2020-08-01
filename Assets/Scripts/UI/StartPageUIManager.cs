 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 代替读取存档功能
public class Save
{
    public static Save ReadSave()   // 找到返回Save，否则返回null
    {
        return new Save();
    }
}

public class StartPageUIManager : MonoBehaviour
{
    private Save save;    // 存档
    public GameObject cover, startClicked_Save, exitClicked;

    void Start()
    {
        // 读取存档
        save = Save.ReadSave();
    }

    // 通用控制
    private void StartGame()
    {
        // 启动游戏的转场
        // 开始新游戏
        //SceneManager.LoadScene("GameScene");
        SceneManager.LoadScene("SampleScene");  // 因为目前游戏主界面叫SampleScene
    }

    private void StartWarningDisplayReverse()
    {
        // 反转警告页显示/隐藏关系
        this.cover.SetActive(!this.cover.activeSelf);   // 封面
        this.startClicked_Save.SetActive(!this.startClicked_Save.activeSelf); // 开始警告
    }

    private void ExitWarningDisplayReverse()
    {
        // 反转退出页显示/隐藏关系
        this.cover.SetActive(!this.cover.activeSelf);   //封面
        this.exitClicked.SetActive(!this.exitClicked.activeSelf);   // 退出警告
    }

    // 封面页控制函数
    public void OnStartBtnClick()
    {
        if (this.save != null) this.StartWarningDisplayReverse();
        else this.StartGame();
    }

    public void OnContinueBtnClick()
    {
        if (this.save != null)
        {
            this.StartGame();   // 其他操作过后填充
        }
        else return;    // 否则点不动
    }

    public void OnExitBtnClick()
    {
        this.ExitWarningDisplayReverse();
    }

    // 开始警告页控制函数
    public void OnStartConfirmBtnClick()
    {
        this.StartGame();
    }

    public void OnStartCancelBtnClick()
    {
        this.StartWarningDisplayReverse();
    }

    // 退出警告页控制函数

    public void OnExitConfirmBtnClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnExitCancelBtnClick()
    {
        this.ExitWarningDisplayReverse();
    }
}
