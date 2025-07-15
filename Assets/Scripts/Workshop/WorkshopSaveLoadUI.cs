// ��������������������������������������������������������������������������������������������������������������������������
// WorkshopSaveLoadUI.cs
// �� UI����ť����/���ش��⹤�����ֵ��̶�·��
// ��������������������������������������������������������������������������������������������������������������������������
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class WorkshopSaveLoadUI : MonoBehaviour
{
    public WorkshopSaveLoad saveLoad;         // ����ű�����
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

        Debug.Log("����·��: " + savePath);
    }
}
