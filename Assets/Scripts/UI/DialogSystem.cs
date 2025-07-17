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

    // 用于记录每个场景的对话是否已经播放过
    private static Dictionary<string, bool> sceneDialogPlayed = new Dictionary<string, bool>();
    
    private List<DialogLine> dialogLines = new List<DialogLine>(); // 存储对话行（英文和中文）
    private bool hasAutoShown = false;// 用于判断是否自动显示了第一句
    private bool dialogFinished = false;// 用于判断对话是否已经结束
    private string currentSceneName; // 当前场景名称

    // 对话行数据结构
    [System.Serializable]
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
        // 获取当前场景名称
        currentSceneName = SceneManager.GetActiveScene().name;
        
        GetTextFromFile(textFile);

        // 检查当前场景的对话是否已经播放过
        if (sceneDialogPlayed.ContainsKey(currentSceneName) && sceneDialogPlayed[currentSceneName])
        {
            // 如果当前场景的对话已经播放过，直接进入游戏状态
            dialogBox.SetActive(false);
            Time.timeScale = 1f;
            return;
        }

        //禁用所有游戏操作
        Time.timeScale = 0f;

        // 自动显示第一句对话
        if (dialogLines.Count > 0)
        {
            dialogBox.SetActive(true);//确保对话框是开启的
            DisplayDialogLine(0);
            index = 1;
            hasAutoShown = true;
            //Debug.Log($"对话开始，总共有 {dialogLines.Count} 句对话，当前显示第 0 句，index = {index}");
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
            if (index < dialogLines.Count)
            {
                DisplayDialogLine(index);
                //Debug.Log($"显示第 {index} 句对话，共 {dialogLines.Count} 句");
                index++;
            }
            else
            {
                //Debug.Log($"对话结束 - index: {index}, dialogLines.Count: {dialogLines.Count}");
                dialogBox.SetActive(false);//文本显示完毕，隐藏对话框

                // 重新允许游戏操作
                Time.timeScale = 1f;
                
                // 标记对话已结束，防止继续响应按键
                dialogFinished = true;
                
                // 标记当前场景的对话已经播放过
                sceneDialogPlayed[currentSceneName] = true;
            }
        }
    }

    // 显示对话行（英文在上，中文在下）
    void DisplayDialogLine(int lineIndex)
    {
        if (lineIndex >= 0 && lineIndex < dialogLines.Count)
        {
            DialogLine line = dialogLines[lineIndex];
            englishTextLabel.text = line.englishText;
            chineseTextLabel.text = line.chineseText;
        }
    }

    void GetTextFromFile(TextAsset file)
    {
        dialogLines.Clear();
        index = 0;

        if (file == null)
        {
            Debug.LogWarning("DialogSystem: 文本文件为空");
            return;
        }

        // 获取原始文本
        string originalText = file.text;
        Debug.Log($"DialogSystem: 原始文本长度: {originalText.Length}");
        Debug.Log($"DialogSystem: 原始文本内容:\n{originalText}");

        // 使用最安全的方法：StringReader逐行读取
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
                    else
                    {
                        Debug.Log($"DialogSystem: 第 {lineNumber} 行为空，已跳过");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"DialogSystem: 读取文本文件时出错: {e.Message}");
            return;
        }
        
        // 解析每行文本，分离英文和中文
        foreach (string line in lines)
        {
            ParseDialogLine(line);
        }
        
        Debug.Log($"DialogSystem: 总共加载了 {dialogLines.Count} 行对话");
        
        // 最终验证
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

    // 解析对话行，分离英文和中文
    void ParseDialogLine(string line)
    {
        // 跳过空行
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        // 查找中文字符，分离英文和中文
        int firstChineseIndex = -1;
        for (int i = 0; i < line.Length; i++)
        {
            if (IsChineseCharacter(line[i]))
            {
                firstChineseIndex = i;
                break;
            }
        }

        if (firstChineseIndex != -1)
        {
            // 找到中文字符，分离英文和中文
            string englishText = line.Substring(0, firstChineseIndex).Trim();
            string chineseText = line.Substring(firstChineseIndex).Trim();
            
            // 如果英文部分为空，可能是纯中文
            if (string.IsNullOrWhiteSpace(englishText))
            {
                dialogLines.Add(new DialogLine("", chineseText));
            }
            else
            {
                dialogLines.Add(new DialogLine(englishText, chineseText));
            }
            return;
        }

        // 如果没有找到中文字符，将整行作为英文处理
        dialogLines.Add(new DialogLine(line.Trim(), ""));
    }

    // 判断字符是否为中文字符
    bool IsChineseCharacter(char c)
    {
        // 中文字符范围：基本汉字、扩展汉字、CJK统一汉字等
        return (c >= 0x4E00 && c <= 0x9FFF) || // 基本汉字
               (c >= 0x3400 && c <= 0x4DBF) || // 扩展A
               (c >= 0x20000 && c <= 0x2A6DF) || // 扩展B
               (c >= 0x2A700 && c <= 0x2B73F) || // 扩展C
               (c >= 0x2B740 && c <= 0x2B81F) || // 扩展D
               (c >= 0x2B820 && c <= 0x2CEAF) || // 扩展E
               (c >= 0xF900 && c <= 0xFAFF) || // 兼容汉字
               (c >= 0x2F800 && c <= 0x2FA1F); // 兼容扩展
    }

    // 重置指定场景的对话播放状态
    public static void ResetSceneDialog(string sceneName)
    {
        if (sceneDialogPlayed.ContainsKey(sceneName))
        {
            sceneDialogPlayed[sceneName] = false;
        }
    }

    // 重置所有场景的对话播放状态
    public static void ResetAllSceneDialogs()
    {
        sceneDialogPlayed.Clear();
    }

    // 检查指定场景的对话是否已播放
    public static bool IsSceneDialogPlayed(string sceneName)
    {
        return sceneDialogPlayed.ContainsKey(sceneName) && sceneDialogPlayed[sceneName];
    }
}
