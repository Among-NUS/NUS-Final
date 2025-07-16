using UnityEngine;
using RuntimeInspectorNamespace;

public class WorkshopHotkeys : MonoBehaviour
{
    public RuntimeHierarchy hierarchy;   // ���� RuntimeHierarchy
    public RuntimeInspector inspector;   // ���� RuntimeInspector
    public Transform spawnRoot;          // ��� SpawnRoot

    private GameObject currentSelected;

    void Update()
    {
        // ��ȡ��ǰ Inspector ѡ�еĶ���
        if (inspector != null)
        {
            if (inspector.InspectedObject is GameObject go)
                currentSelected = go;
            else
                currentSelected = null;
        }

        // Delete ��ɾ��
        if (currentSelected != null && Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteCurrent();
        }
    }

    private void DeleteCurrent()
    {
        Debug.Log($"[Hotkeys] Delete {currentSelected.name}");

        // ȷ��ֻɾ�� SpawnRoot �µ�
        if (currentSelected.transform.IsChildOf(spawnRoot))
        {
            Destroy(currentSelected);
            hierarchy?.Refresh();
            inspector?.Inspect(null);
        }
        else
        {
            Debug.LogWarning("[Hotkeys] ѡ�еĲ��� SpawnRoot �µ����壬����ɾ��");
        }
    }
}
