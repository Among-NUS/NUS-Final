using UnityEngine;
using RuntimeInspectorNamespace;
using System.Collections.ObjectModel;

public class GizmoBridge : MonoBehaviour
{
    public RuntimeHierarchy hierarchy;
    public RuntimeInspector inspector;
    public Camera gizmoCamera;    // ✅ 你的编辑相机
    public LayerMask selectableMask = ~0;  // 选中的层（默认全部）

    private GameObject lastSelected;

    void Awake()
    {
        if (hierarchy != null)
            hierarchy.OnSelectionChanged += OnSelectionChanged;
    }

    void Update()
    {
        // ✅ 监听鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            TrySelectUnderMouse();
        }
    }

    private void TrySelectUnderMouse()
    {
        if (gizmoCamera == null)
        {
            Debug.LogWarning("[GizmoBridge] 没有指定 gizmoCamera");
            return;
        }

        Ray ray = gizmoCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, selectableMask))
        {
            GameObject hitGO = hit.collider.gameObject;
            Debug.Log($"[GizmoBridge] 点击命中 {hitGO.name}");

            // 找 Wrapper（命中的可能是子物体，需要找父节点 Wrapper）
            Transform wrapper = hitGO.transform;
            while (wrapper.parent != null && !wrapper.name.Contains("_Wrapper"))
                wrapper = wrapper.parent;

            if (wrapper != null)
            {
                lastSelected = wrapper.gameObject;

                // ✅ 同步给 Inspector & Hierarchy
                inspector?.Inspect(lastSelected);
                hierarchy?.Select(wrapper);
            }
        }
    }

    void OnSelectionChanged(ReadOnlyCollection<Transform> selection)
    {
        if (selection != null && selection.Count > 0)
        {
            Transform selected = selection[^1];
            if (selected != null)
            {
                inspector?.Inspect(selected.gameObject);
                lastSelected = selected.gameObject;
            }
        }
        else
        {
            inspector?.Inspect(null);
            lastSelected = null;
        }
    }
}
