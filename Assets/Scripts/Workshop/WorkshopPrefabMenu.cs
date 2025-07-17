using UnityEngine;
using UnityEngine.UI;
using TMPro; // 兼容 TMP_Text

public class WorkshopPrefabMenu : MonoBehaviour
{
    [Header("UI References")]
    public Transform buttonContainer;      // ScrollView/Viewport/Content
    public Button buttonTemplate;          // 按钮模板（隐藏）
    public string prefabFolder = "Prefabs/Workshop";
    public WorkshopManager workshopManager;

    void Start()
    {
        // 检查 Content 是否有布局组件
        EnsureLayout(buttonContainer.gameObject);

        // 扫描 Prefab
        var prefabs = Resources.LoadAll<GameObject>(prefabFolder);
        Debug.Log($"[PrefabMenu] 找到 {prefabs.Length} 个 Prefab");

        if (prefabs.Length == 0) return;

        foreach (var prefab in prefabs)
        {
            // ✅ 跳过 Hero 和 LevelExit
            if (prefab.name == "Hero" || prefab.name == "LevelExit")
            {
                Debug.Log($"[PrefabMenu] 跳过特殊物体 {prefab.name}");
                continue;
            }

            CreateButtonForPrefab(prefab.name);
        }

        // 最后隐藏模板
        buttonTemplate.gameObject.SetActive(false);
    }

    void CreateButtonForPrefab(string prefabName)
    {
        var btnObj = Instantiate(buttonTemplate, buttonContainer);
        btnObj.gameObject.SetActive(true);

        // 支持 TextMeshPro
        var textComp = btnObj.GetComponentInChildren<Text>();
        if (textComp) textComp.text = prefabName;
        else
        {
            var tmpText = btnObj.GetComponentInChildren<TMP_Text>();
            if (tmpText) tmpText.text = prefabName;
        }

        btnObj.onClick.AddListener(() =>
        {
            Debug.Log($"[PrefabMenu] 点击生成 {prefabName}");
            workshopManager.SpawnPrefab(prefabName);
        });
    }

    void EnsureLayout(GameObject content)
    {
        // 如果没有 VerticalLayoutGroup，自动添加
        var vlg = content.GetComponent<VerticalLayoutGroup>();
        if (!vlg)
        {
            vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 10;
        }

        // 如果没有 ContentSizeFitter，自动添加
        var csf = content.GetComponent<ContentSizeFitter>();
        if (!csf)
        {
            csf = content.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }
}
