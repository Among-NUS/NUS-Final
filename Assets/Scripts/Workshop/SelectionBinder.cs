using UnityEngine;
using RuntimeInspectorNamespace;

public class SelectionBinder : MonoBehaviour
{
    public RuntimeHierarchy hierarchy;
    public RuntimeSelectionHighlighter highlighter;

    void Awake()
    {
        hierarchy.OnSelectionChanged += OnSelChanged;
    }

    void OnDestroy()
    {
        hierarchy.OnSelectionChanged -= OnSelChanged;
    }

    void OnSelChanged(System.Collections.ObjectModel.ReadOnlyCollection<Transform> sel)
    {
        if (sel.Count == 0 || sel[0] == null)
        {
            highlighter.Clear();
            return;
        }

        // 若点到子物体，向上找 "_Wrapper"
        Transform t = sel[0];
        while (t && !t.name.EndsWith("_Wrapper"))
            t = t.parent;

        highlighter.Highlight(t ? t.gameObject : null);
    }
}
