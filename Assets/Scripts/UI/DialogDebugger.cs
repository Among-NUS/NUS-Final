using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 对话调试器 - 帮助诊断文本分割问题
/// </summary>
public class DialogDebugger : MonoBehaviour
{
    [Header("调试设置")]
    public TextAsset debugTextFile;
    public bool debugOnStart = true;
    
    void Start()
    {
        if (debugOnStart && debugTextFile != null)
        {
            DebugTextFile();
        }
    }
    
    /// <summary>
    /// 调试文本文件
    /// </summary>
    public void DebugTextFile()
    {
        if (debugTextFile == null)
        {
            Debug.LogWarning("DialogDebugger: 未指定调试文本文件");
            return;
        }
        
        Debug.Log("=== 开始调试文本文件 ===");
        
        string text = debugTextFile.text;
        
        // 显示原始文本信息
        Debug.Log($"文本文件长度: {text.Length}");
        Debug.Log($"文本文件内容:\n{text}");
        
        // 分析字符
        AnalyzeText(text);
        
        // 测试不同的分割方法
        TestSplitMethods(text);
        
        Debug.Log("=== 调试完成 ===");
    }
    
    /// <summary>
    /// 分析文本内容
    /// </summary>
    void AnalyzeText(string text)
    {
        Debug.Log("=== 文本分析 ===");
        
        // 统计各种字符
        int newlines = 0;
        int commas = 0;
        int spaces = 0;
        int tabs = 0;
        
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            switch (c)
            {
                case '\n': newlines++; break;
                case '\r': newlines++; break;
                case ',': commas++; break;
                case ' ': spaces++; break;
                case '\t': tabs++; break;
            }
        }
        
        Debug.Log($"换行符数量: {newlines}");
        Debug.Log($"逗号数量: {commas}");
        Debug.Log($"空格数量: {spaces}");
        Debug.Log($"制表符数量: {tabs}");
        
        // 显示每个字符的ASCII码
        Debug.Log("=== 字符分析 ===");
        for (int i = 0; i < Mathf.Min(text.Length, 100); i++) // 只显示前100个字符
        {
            char c = text[i];
            Debug.Log($"位置 {i}: '{c}' (ASCII: {(int)c})");
        }
    }
    
    /// <summary>
    /// 测试不同的分割方法
    /// </summary>
    void TestSplitMethods(string text)
    {
        Debug.Log("=== 测试分割方法 ===");
        
        // 方法1: 使用StringReader
        Debug.Log("方法1: StringReader");
        List<string> lines1 = SplitWithStringReader(text);
        for (int i = 0; i < lines1.Count; i++)
        {
            Debug.Log($"  行 {i + 1}: \"{lines1[i]}\"");
        }
        
        // 方法2: 使用Split('\n')
        Debug.Log("方法2: Split('\\n')");
        List<string> lines2 = SplitWithNewline(text);
        for (int i = 0; i < lines2.Count; i++)
        {
            Debug.Log($"  行 {i + 1}: \"{lines2[i]}\"");
        }
        
        // 方法3: 使用Split(',')
        Debug.Log("方法3: Split(',')");
        List<string> lines3 = SplitWithComma(text);
        for (int i = 0; i < lines3.Count; i++)
        {
            Debug.Log($"  行 {i + 1}: \"{lines3[i]}\"");
        }
    }
    
    /// <summary>
    /// 使用StringReader分割
    /// </summary>
    List<string> SplitWithStringReader(string text)
    {
        List<string> lines = new List<string>();
        using (System.IO.StringReader reader = new System.IO.StringReader(text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    lines.Add(trimmed);
                }
            }
        }
        return lines;
    }
    
    /// <summary>
    /// 使用换行符分割
    /// </summary>
    List<string> SplitWithNewline(string text)
    {
        List<string> lines = new List<string>();
        string[] splitLines = text.Split('\n');
        
        foreach (var line in splitLines)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                lines.Add(trimmed);
            }
        }
        return lines;
    }
    
    /// <summary>
    /// 使用逗号分割
    /// </summary>
    List<string> SplitWithComma(string text)
    {
        List<string> lines = new List<string>();
        string[] splitLines = text.Split(',');
        
        foreach (var line in splitLines)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                lines.Add(trimmed);
            }
        }
        return lines;
    }
    
    /// <summary>
    /// 公共方法：手动调试
    /// </summary>
    [ContextMenu("调试文本文件")]
    public void DebugManually()
    {
        DebugTextFile();
    }
} 