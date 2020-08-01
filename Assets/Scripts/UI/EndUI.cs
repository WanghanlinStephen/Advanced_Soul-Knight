using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndUI : MonoBehaviour
{
    public GameObject end;

    public void OnGameEnd()
    {
        this.SwitchUI();
    }

    public void OnBackToStartPageBtnClick()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void OnRestartClick()
    {
        SceneManager.LoadScene("SampleScene");  // 游戏主体现名
    }

    public void SwitchUI()
    {
        //GameObject.Find("PauseUI").SetActive(false);
        this.transform.parent.gameObject.transform.SetAsLastSibling();  // 把自己放到最后，否则会被其他UI覆盖
        end.SetActive(true);
        if (end.activeSelf)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
