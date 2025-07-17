using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;

public class WorkshopSaveLoadUI : MonoBehaviour
{
    [Header("Core")]
    public WorkshopSaveLoad saveLoad;

    [Header("UI Buttons")]
    public Button saveButton;
    public Button loadButton;

    [Header("TMP Input")]
    public TMP_InputField fileNameField;

    [Header("可选提示 UI")]
    public TMP_Text hintText; // ✅ 提示显示区域

    void Start()
    {
        // ✅ 默认文件名 .ana
        if (fileNameField != null && string.IsNullOrEmpty(fileNameField.text))
            fileNameField.text = "WorkshopLevel.ana";

        if (saveButton != null)
            saveButton.onClick.AddListener(() =>
            {
                string name = GetFileNameOrDefault();
                saveLoad.SaveLayout(name);

                string fullPath = WorkshopSaveLoad.ResolvePath(name); // 需要 public static
                ShowHint($"Saved to:\n{fullPath}");
            });

        if (loadButton != null)
            loadButton.onClick.AddListener(() =>
            {
                string name = GetFileNameOrDefault();
                saveLoad.LoadLayoutForEditor(name);

                string fullPath = WorkshopSaveLoad.ResolvePath(name);
                ShowHint($"Loaded from:\n{fullPath}");
            });

        Debug.Log("✅ WorkshopSaveLoadUI 初始化完成");
    }

    string GetFileNameOrDefault()
    {
        return (fileNameField != null && !string.IsNullOrWhiteSpace(fileNameField.text))
            ? fileNameField.text.Trim()
            : "WorkshopLevel.ana";
    }

    /// ✅ 显示提示并在 1 秒后自动隐藏
    void ShowHint(string msg)
    {
        Debug.Log(msg);

        if (hintText)
        {
            hintText.text = msg;
            StopAllCoroutines();              // 避免多次点击叠加
            StartCoroutine(HideHintAfterDelay(1f));
        }
    }

    IEnumerator HideHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hintText) hintText.text = "";
    }
}
