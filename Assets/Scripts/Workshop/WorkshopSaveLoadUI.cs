// ─────────────────────────────────────────────────────────────
// WorkshopSaveLoadUI.cs
// 简单 UI：按钮保存/加载创意工坊布局到固定路径
// ─────────────────────────────────────────────────────────────
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class WorkshopSaveLoadUI : MonoBehaviour
{
    public WorkshopSaveLoad saveLoad;         // 拖入脚本引用
    public Button saveButton;
    public Button loadButton;

    private string savePath;

    void Start()
    {
        string savePath = "WorkshopLevel.json";

        if (saveButton != null)
            saveButton.onClick.AddListener(() => saveLoad.SaveLayout(savePath));

        if (loadButton != null)
            loadButton.onClick.AddListener(() => saveLoad.LoadLayoutForEditor(savePath));

        Debug.Log("保存路径: " + savePath);
    }
}
