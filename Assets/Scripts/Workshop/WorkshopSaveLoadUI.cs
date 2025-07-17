using UnityEngine;
using UnityEngine.UI;
using TMPro;   // ✅ 引入 TMP 命名空间

public class WorkshopSaveLoadUI : MonoBehaviour
{
    [Header("Core")]
    public WorkshopSaveLoad saveLoad;

    [Header("UI Buttons")]
    public Button saveButton;
    public Button loadButton;

    [Header("TMP Input")]
    public TMP_InputField fileNameField;  // ✅ 改为 TMP 版本

    void Start()
    {
        // 默认文件名
        if (fileNameField != null && string.IsNullOrEmpty(fileNameField.text))
            fileNameField.text = "WorkshopLevel.json";

        if (saveButton != null)
            saveButton.onClick.AddListener(() =>
            {
                string name = (fileNameField != null && !string.IsNullOrWhiteSpace(fileNameField.text))
                              ? fileNameField.text.Trim()
                              : "WorkshopLevel.json";

                saveLoad.SaveLayout(name);
            });

        if (loadButton != null)
            loadButton.onClick.AddListener(() =>
            {
                string name = (fileNameField != null && !string.IsNullOrWhiteSpace(fileNameField.text))
                              ? fileNameField.text.Trim()
                              : "WorkshopLevel.json";

                saveLoad.LoadLayoutForEditor(name);
            });

        Debug.Log("✅ WorkshopSaveLoadUI 初始化完成");
    }
}
