using UnityEngine;
using UnityEngine.UI;

public class PrefabSelectorUI : MonoBehaviour
{
    public Dropdown prefabDropdown;           // 绑定 UI Dropdown
    public WorkshopManager workshopManager;   // 绑定你的 WorkshopManager

    private GameObject[] allPrefabs;          // 缓存扫描到的 Prefab

    void Start()
    {
        LoadPrefabList();
    }

    void LoadPrefabList()
    {
        // ✅ 扫描 Resources/Prefabs/Workshop 目录
        allPrefabs = Resources.LoadAll<GameObject>("Prefabs/Workshop");

        prefabDropdown.ClearOptions();

        if (allPrefabs.Length == 0)
        {
            prefabDropdown.options.Add(new Dropdown.OptionData("⚠️ 没有找到任何 Prefab"));
            prefabDropdown.interactable = false;
            Debug.LogWarning("⚠️ 没有在 Resources/Prefabs/Workshop/ 找到任何 Prefab");
            return;
        }

        // ✅ 把所有 Prefab 名字列到 Dropdown
        foreach (var prefab in allPrefabs)
        {
            prefabDropdown.options.Add(new Dropdown.OptionData(prefab.name));
        }

        prefabDropdown.interactable = true;

        // ✅ 默认选第一个
        prefabDropdown.value = 0;
        prefabDropdown.RefreshShownValue();

        // ✅ 立即同步给 WorkshopManager
        if (workshopManager != null)
            workshopManager.prefabToSpawn = allPrefabs[0];

        // ✅ 监听选择变化
        prefabDropdown.onValueChanged.AddListener(OnPrefabSelected);

        Debug.Log($"✅ 已扫描到 {allPrefabs.Length} 个 Prefab → 下拉菜单已更新");
    }

    void OnPrefabSelected(int index)
    {
        if (index < 0 || index >= allPrefabs.Length)
        {
            Debug.LogWarning("⚠️ Prefab 索引超出范围");
            return;
        }

        if (workshopManager != null)
        {
            workshopManager.prefabToSpawn = allPrefabs[index];
            Debug.Log($"✅ 选择 Prefab：{allPrefabs[index].name}");
        }
    }
}
