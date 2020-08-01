using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public GameObject pause, pauseTrigger;

    void Start()
    {
        this.SwitchUI();
        this.SwitchUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.SwitchUI();
        }
    }

    public void OnPauseBtnClick()
    {
        this.SwitchUI();
    }

    public void OnBackToStartPageBtnClick()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void OnContinueBtnClick()
    {
        this.SwitchUI();
    }

    public void SwitchUI()
    {
        pauseTrigger.SetActive(!pauseTrigger.activeSelf);
        pause.SetActive(!pause.activeSelf);
        if (pause.activeSelf)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
