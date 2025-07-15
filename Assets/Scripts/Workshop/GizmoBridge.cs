using UnityEngine;
using RuntimeInspectorNamespace;
using System.Collections.ObjectModel;

public class GizmoBridge : MonoBehaviour
{
    public RuntimeHierarchy hierarchy;
    public RuntimeInspector inspector;

    void Awake()
    {
        if (hierarchy != null)
            hierarchy.OnSelectionChanged += OnSelectionChanged;
    }

    void OnSelectionChanged(ReadOnlyCollection<Transform> selection)
    {
        if (selection != null && selection.Count > 0)
        {
            Transform selected = selection[^1];
            if (selected != null)
                inspector?.Inspect(selected.gameObject);
        }
        else
        {
            inspector?.Inspect(null);
        }

        // ❌ 不再控制 gizmo（当前版本不支持）:
        // gizmo.Target = ...;
        // gizmo.SetTransformTarget(...);
    }
}
