using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogSystem : MonoBehaviour
{
    [Header("UI Component")]
    public TextMeshProUGUI textlabel;
   // public Image faceImage;
    public GameObject dialogBox;

    [Header("Text File")]
    public TextAsset textFile;
    public int index;

    [Header("Character Control")]
    public MonoBehaviour playerController; // 控制hero移动的脚本

    private List<string> textlist = new List<string>();
    private bool hasAutoShown = false;// 用于判断是否自动显示了第一句

    void Start()
    {
        GetTextFromFile(textFile);

        if (playerController != null)
        {
            playerController.enabled = false; // 禁止角色移动
        }

        // 自动显示第一句对话
        if (textlist.Count > 0)
        {
            dialogBox.SetActive(true);//确保对话框是开启的
            textlabel.text = textlist[0];
            index = 1;
            hasAutoShown = true;
        }
        else
        {
            dialogBox.SetActive(false); // 如果文本为空，自动隐藏
        }
    }

    void Update()
    {
        // 只有在第一句已经显示之后，才响应按键显示后续内容
        //按任意按键切换
        if (hasAutoShown && Input.anyKeyDown)
        {
            if (index < textlist.Count)
            {
                textlabel.text = textlist[index];
                index++;
            }
            else
            {
                Debug.Log("对话结束");
                dialogBox.SetActive(false);//文本显示完毕，隐藏对话框

                // 重新允许hero移动
                if (playerController != null)
                {
                    playerController.enabled = true;
                }
            }
        }
    }

    void GetTextFromFile(TextAsset file)
    {
        textlist.Clear();
        index = 0;

        var lineData = file.text.Split('\n');
        foreach (var line in lineData)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                textlist.Add(trimmed);// 去除多余空格或换行
            }
        }
    }
}
