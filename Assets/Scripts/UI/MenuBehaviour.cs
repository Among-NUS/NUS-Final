using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    public GameObject StartPanel;
    public GameObject LevelPanel;

    public void OnClickStart()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void OnClickExit()
    {
        Debug.Log("quitGame");
        Application.Quit();
    }
    public void OnClickLevel()
    {
        StartPanel.SetActive(false);
        LevelPanel.SetActive(true);
    }
    public void OnClickWorkshop()
    {
        SceneManager.LoadScene("EditScene");
    }
    public void OnClickCertainLevel(int levelNo)
    {
        switch (levelNo) {
            case 0:
                SceneManager.LoadScene("Tutorial");
                break;
            case 1:
                SceneManager.LoadScene("Tutorial2");
                break;
            case 2:
                SceneManager.LoadScene("Level_2");
                break;
            case 3:
                SceneManager.LoadScene("Level1Real");
                break;
            case 4:
                SceneManager.LoadScene("Level3");
                break;
            case 5:
                SceneManager.LoadScene("Level_4");
                break;
        }
    }
    private void Start()
    {
        StartPanel.SetActive(true);
        LevelPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            StartPanel.SetActive(true);
            LevelPanel.SetActive(false);
        }
    }

}
