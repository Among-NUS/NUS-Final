using UnityEngine;
using UnityEngine.EventSystems;
using RuntimeInspectorNamespace;

/// <summary>
/// 将 Scene/Game 视图中的鼠标点击与 RuntimeHierarchy 选中状态保持同步
/// </summary>
public class SceneHierarchySync : MonoBehaviour
{
    [Tooltip("运行时层级面板组件")]
    public RuntimeHierarchy hierarchy;

    [Tooltip("用于射线检测的相机 (一般填 Gizmo 相机)")]
    public Camera gizmoCamera;

    [Tooltip("哪些 Layer 参与点击检测")]
    public LayerMask pickableLayers = ~0;   // 默认全部

    void Update()
    {
        // 左键点击且鼠标不在 UI 上
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = gizmoCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, pickableLayers))
            {
                // 找到顶层 "_Wrapper" 物体
                Transform t = hit.collider.transform;
                while (t && !t.name.EndsWith("_Wrapper"))
                    t = t.parent;

                if (t != null && hierarchy != null)
                {
                    hierarchy.Select(t, RuntimeHierarchy.SelectOptions.None);
                }
            }
        }
    }
}
