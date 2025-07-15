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


    //用于记录对话是否已经播放过
    private static bool hasDialogBeenShown = false;
    
    private List<string> textlist = new List<string>();
    private bool hasAutoShown = false;// 用于判断是否自动显示了第一句
    private bool dialogFinished = false;// 用于判断对话是否已经结束

    void Start()
    {
        GetTextFromFile(textFile);

        // 如果对话已经播放过，直接进入游戏状态
        if (hasDialogBeenShown)
        {
            dialogBox.SetActive(false);
            Time.timeScale = 1f;
            return;
        }

        //禁用所有游戏操作
        Time.timeScale = 0f;

        // 自动显示第一句对话
        if (textlist.Count > 0)
        {
            dialogBox.SetActive(true);//确保对话框是开启的
            textlabel.text = textlist[0];
            index = 1;
            hasAutoShown = true;
            //Debug.Log($"对话开始，总共有 {textlist.Count} 句对话，当前显示第 0 句，index = {index}");
        }
        else
        {
            dialogBox.SetActive(false); // 如果文本为空，自动隐藏
            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        // 确保在对话期间 Time.timeScale 保持为 0
        if (hasAutoShown && !dialogFinished && Time.timeScale != 0)
        {
            Time.timeScale = 0f;
        }
        
        // 只有在第一句已经显示之后，且对话未结束时，才响应按键显示后续内容
        //按任意按键切换
        if (hasAutoShown && !dialogFinished && Input.anyKeyDown)
        {
            if (index < textlist.Count)
            {
                textlabel.text = textlist[index];
                //Debug.Log($"显示第 {index} 句对话，共 {textlist.Count} 句");
                index++;
            }
            else
            {
                //Debug.Log($"对话结束 - index: {index}, textlist.Count: {textlist.Count}");
                dialogBox.SetActive(false);//文本显示完毕，隐藏对话框

                // 重新允许游戏操作
                Time.timeScale = 1f;
                
                // 标记对话已结束，防止继续响应按键
                dialogFinished = true;
                
                // 标记对话已经播放过，下次重启时不再显示
                hasDialogBeenShown = true;
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
