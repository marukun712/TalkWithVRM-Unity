using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject Chair;

    //シーン遷移

    public void LoadNormalMode()
    {
        SceneManager.LoadScene("TalkWithVRM_Chat");
    }

    public void LoadTalkMode()
    {
        SceneManager.LoadScene("TalkMode");
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
