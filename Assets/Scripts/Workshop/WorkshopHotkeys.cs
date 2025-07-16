using UnityEngine;
using RuntimeInspectorNamespace;

public class WorkshopHotkeys : MonoBehaviour
{
    public RuntimeHierarchy hierarchy;   // 拖入 RuntimeHierarchy
    public RuntimeInspector inspector;   // 拖入 RuntimeInspector
    public Transform spawnRoot;          // 你的 SpawnRoot

    private GameObject currentSelected;

    void Update()
    {
        // 获取当前 Inspector 选中的对象
        if (inspector != null)
        {
            if (inspector.InspectedObject is GameObject go)
                currentSelected = go;
            else
                currentSelected = null;
        }

        // Delete 键删除
        if (currentSelected != null && Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteCurrent();
        }
    }

    private void DeleteCurrent()
    {
        Debug.Log($"[Hotkeys] Delete {currentSelected.name}");

        // 确保只删除 SpawnRoot 下的
        if (currentSelected.transform.IsChildOf(spawnRoot))
        {
            Destroy(currentSelected);
            hierarchy?.Refresh();
            inspector?.Inspect(null);
        }
        else
        {
            Debug.LogWarning("[Hotkeys] 选中的不是 SpawnRoot 下的物体，跳过删除");
        }
    }
}
