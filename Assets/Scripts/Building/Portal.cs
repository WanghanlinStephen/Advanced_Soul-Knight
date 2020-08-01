using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Portal : MonoBehaviour, IStructure
{
    private Animator anim;
    // 是否已经启动
    private bool start;

    void Start()
    {
        start = false;
        anim = GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (Input.GetKey(KeyCode.E) && other.CompareTag("Player") && !start)
        {
            start = true;
            anim.SetBool("Run", true);
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait(){
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("SampleScene");
        anim.SetBool("Run", false);
    }
}
