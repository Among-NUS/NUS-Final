using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class DialogSystem : MonoBehaviour
{
    [Header("UI Component")]
    public TextMeshProUGUI englishTextLabel; // 英文文本组件
    public TextMeshProUGUI chineseTextLabel; // 中文文本组件
   // public Image faceImage;
    public GameObject dialogBox;

    [Header("Text File")]
    public TextAsset textFile;
    public int index;

    private List<DialogLine> dialogLines = new List<DialogLine>(); // 存储对话行
    private bool hasAutoShown = false; // 是否已自动显示第一句
    private bool dialogFinished = false; // 对话是否已完成
    private string currentSceneName; // 当前场景名称

   
    private static Dictionary<string, bool> sceneDialogPlayed = new Dictionary<string, bool>();// 存储每个场景的对话播放状态

    [Serializable]
    public class DialogLine
    {
        public string englishText;
        public string chineseText;
        
        public DialogLine(string english, string chinese)
        {
            englishText = english;
            chineseText = chinese;
        }
    }

    void Start()
    {
        StartDialog();
    }

    void Update()
    {
        HandleDialogInput();
    }

 
    public void StartDialog(bool resetDialogState = false)
    {
        InitializeDialog();
        GetTextFromFile(textFile);
        
        if (ShouldSkipDialog(resetDialogState))// 检查是否需要跳过对话
        {
            SkipDialog();
            return;
        }

        BeginDialog();
    }

 
    //检查对话是否已完成
   public bool IsDialogFinished()
    {
        return dialogFinished;
    }

     
    //检查指定场景的对话是否已播放
    public static bool IsSceneDialogPlayed(string sceneName)
    {
        return sceneDialogPlayed.ContainsKey(sceneName) && sceneDialogPlayed[sceneName];
    }


    //重置指定场景的对话播放状态
       public static void ResetSceneDialog(string sceneName)
    {
        if (sceneDialogPlayed.ContainsKey(sceneName))// 如果字典中已存在该场景，则重置其状态
        {
            sceneDialogPlayed[sceneName] = false;
        }
        else 
        {// 如果字典中不存在该场景，则添加并设置状态为未播放
            sceneDialogPlayed[sceneName] = false;
        }
    }


    //重置所有场景的对话播放状态
      public static void ResetAllSceneDialogs()
    {
        sceneDialogPlayed.Clear();
    }


    /// 标记指定场景的对话为已播放
    public static void MarkSceneDialogAsPlayed(string sceneName)
    {
        sceneDialogPlayed[sceneName] = true;
    }

  
    private void InitializeDialog()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        dialogFinished = false;
        hasAutoShown = false;
        index = 0;
    }
 
    //判断是否应该跳过对话
     private bool ShouldSkipDialog(bool resetDialogState)
    {
        if (resetDialogState)
        {
            ResetSceneDialog(currentSceneName);
            return false;
        }
        
        return IsSceneDialogPlayed(currentSceneName);
    }

      private void SkipDialog()
    {
        dialogBox.SetActive(false);
        Time.timeScale = 1f;
    }

  
    private void BeginDialog()
    {
        Time.timeScale = 0f; // 暂停游戏

        if (dialogLines.Count > 0)
        {
            dialogBox.SetActive(true);
            DisplayDialogLine(0);
            index = 1;
            hasAutoShown = true;
        }
        else
        {
            SkipDialog();
        }
    }

 
    private void HandleDialogInput()
    {
        // 确保在对话期间时间暂停
        if (hasAutoShown && !dialogFinished && Time.timeScale != 0)
        {
            Time.timeScale = 0f;
        }
        
        // 处理按键输入
        if (hasAutoShown && !dialogFinished && Input.anyKeyDown)
        {
            ProcessNextDialog();
        }
    }


    private void ProcessNextDialog()
    {
        if (index < dialogLines.Count)
        {
            DisplayDialogLine(index);
            index++;
        }
        else
        {
            FinishDialog();
        }
    }

    private void FinishDialog()
    {
        dialogBox.SetActive(false);
        Time.timeScale = 1f;
        dialogFinished = true;
        MarkSceneDialogAsPlayed(currentSceneName);
    }


    // 显示指定行的对话
       private void DisplayDialogLine(int lineIndex)
    {
        if (lineIndex >= 0 && lineIndex < dialogLines.Count)
        {
            DialogLine line = dialogLines[lineIndex];
            englishTextLabel.text = line.englishText;
            chineseTextLabel.text = line.chineseText;
        }
    }


 
    // 从文件获取文本内容
    private void GetTextFromFile(TextAsset file)
    {
        dialogLines.Clear();

        if (file == null)
        {
            Debug.LogWarning("DialogSystem: 文本文件为空");
            return;
        }

        string originalText = file.text;
        List<string> lines = ParseFileLines(originalText);
        
        foreach (string line in lines)
        {
            ParseDialogLine(line);
        }
        
        LogDialogInfo();
    }

    //解析文本行
    private List<string> ParseFileLines(string originalText)
    {
        List<string> lines = new List<string>();
        
        try
        {
            using (System.IO.StringReader reader = new System.IO.StringReader(originalText))
            {
                string line;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    string trimmed = line.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        lines.Add(trimmed);
                        Debug.Log($"DialogSystem: 第 {lineNumber} 行: \"{trimmed}\"");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"DialogSystem: 读取文本文件时出错: {e.Message}");
        }
        
        return lines;
    }

 
    //解析对话行
    private void ParseDialogLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        int firstChineseIndex = FindFirstChineseCharacter(line);

        if (firstChineseIndex != -1)
        {
            string englishText = line.Substring(0, firstChineseIndex).Trim();
            string chineseText = line.Substring(firstChineseIndex).Trim();
            
            if (string.IsNullOrWhiteSpace(englishText))
            {
                dialogLines.Add(new DialogLine("", chineseText));
            }
            else
            {
                dialogLines.Add(new DialogLine(englishText, chineseText));
            }
        }
        else
        {
            dialogLines.Add(new DialogLine(line.Trim(), ""));
        }
    }


    //查找第一个中文字符的位置
    private int FindFirstChineseCharacter(string line)
    {
        for (int i = 0; i < line.Length; i++)
        {
            if (IsChineseCharacter(line[i]))
            {
                return i;
            }
        }
        return -1;
    }


    //判断字符是否为中文字符
       private bool IsChineseCharacter(char c)
    {
        return (c >= 0x4E00 && c <= 0x9FFF) || // 基本汉字
               (c >= 0x3400 && c <= 0x4DBF) || // 扩展A
               (c >= 0x20000 && c <= 0x2A6DF) || // 扩展B
               (c >= 0x2A700 && c <= 0x2B73F) || // 扩展C
               (c >= 0x2B740 && c <= 0x2B81F) || // 扩展D
               (c >= 0x2B820 && c <= 0x2CEAF) || // 扩展E
               (c >= 0xF900 && c <= 0xFAFF) || // 兼容汉字
               (c >= 0x2F800 && c <= 0x2FA1F); // 兼容扩展
    }


    private void LogDialogInfo()
    {
        Debug.Log($"DialogSystem: 总共加载了 {dialogLines.Count} 行对话");
        
        if (dialogLines.Count == 0)
        {
            Debug.LogWarning("DialogSystem: 没有加载到任何对话行，请检查文本文件格式");
        }
        else
        {
            Debug.Log("DialogSystem: 最终加载的对话内容:");
            for (int i = 0; i < dialogLines.Count; i++)
            {
                DialogLine dialogLine = dialogLines[i];
                Debug.Log($"  第 {i + 1} 句: 英文=\"{dialogLine.englishText}\", 中文=\"{dialogLine.chineseText}\"");
            }
        }
    }
}
