using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    public void ClickStart()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void ClickExit()
    {
        Debug.Log("quitGame");
        Application.Quit();
    }
}
