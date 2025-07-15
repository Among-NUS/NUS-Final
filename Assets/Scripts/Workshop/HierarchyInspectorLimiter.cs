using System.Collections.Generic;
using UnityEngine;
using RuntimeInspectorNamespace;

public class HierarchyInspectorSetup : MonoBehaviour
{
    [Header("Runtime UI")]
    public RuntimeHierarchy hierarchy;   // 拖入 RuntimeHierarchy
    public RuntimeInspector inspector;   // 拖入 RuntimeInspector

    [Header("Root displayed in hierarchy")]
    public Transform spawnRoot;            // 拖入 SpawnRoot

    void Start()
    {
        /* 1) 只显示 SpawnRoot 及其子节点 */
        hierarchy.GameObjectFilter = (Transform t) =>
        {
            // 1) 必须在 SpawnRoot 下面
            bool underRoot = (t == spawnRoot || t.IsChildOf(spawnRoot));

            // 2) 不是高亮线框对象（Edge_0 ~ Edge_11）
            bool notEdge = !t.name.StartsWith("Edge_");

            return underRoot && notEdge;
        };


        /* 2) 绑定 Inspector 与 Hierarchy */
        hierarchy.ConnectedInspector = inspector;

        /* 3) 过滤掉辅助组件(BoxCollider、LineRenderer、WrapperHighlightGizmo) */
        inspector.ComponentFilter = FilterComponents;
    }

    /* 签名必须是 (GameObject, List<Component>) 且返回 void */
    void FilterComponents(GameObject go, List<Component> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            Component c = list[i];

            // 隐藏所有不需暴露的组件
            if (c is BoxCollider ||   // 3D 碰撞箱
                c is BoxCollider2D ||   // 2D 碰撞箱
                c is LineRenderer ||   // 高亮线框
                c is Transform ||   // Transform 组件本身
                c is TransformLock2D)      // 网格锁定脚本
            {
                list.RemoveAt(i);
            }
        }
    }
}
