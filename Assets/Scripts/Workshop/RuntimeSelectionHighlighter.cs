using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class RuntimeSelectionHighlighter : MonoBehaviour
{
    public Color lineColor = Color.yellow;
    public float lineWidth = 0.03f;

    readonly List<GameObject> edges = new();   // 存放 12 条边
    GameObject currentTarget;

    Material lineMat;

    void Awake()
    {
        lineMat = new Material(Shader.Find("Sprites/Default"))
        {
            color = lineColor,
            hideFlags = HideFlags.HideAndDontSave
        };
    }

    public void Highlight(GameObject wrapper)
    {
        Clear();

        currentTarget = wrapper;
        if (currentTarget == null) return;

        if (!currentTarget.TryGetComponent(out BoxCollider box)) return;

        // 计算 8 个顶点（局部坐标）
        Vector3 c = box.center;
        Vector3 s = box.size * 0.5f;

        Vector3[] p =
        {
            new(c.x - s.x, c.y - s.y, c.z - s.z),
            new(c.x + s.x, c.y - s.y, c.z - s.z),
            new(c.x + s.x, c.y + s.y, c.z - s.z),
            new(c.x - s.x, c.y + s.y, c.z - s.z),
            new(c.x - s.x, c.y - s.y, c.z + s.z),
            new(c.x + s.x, c.y - s.y, c.z + s.z),
            new(c.x + s.x, c.y + s.y, c.z + s.z),
            new(c.x - s.x, c.y + s.y, c.z + s.z)
        };

        int[,] e =
        {
            {0,1},{1,2},{2,3},{3,0}, // 底
            {4,5},{5,6},{6,7},{7,4}, // 顶
            {0,4},{1,5},{2,6},{3,7}  // 竖
        };

        for (int i = 0; i < 12; i++)
        {
            GameObject edgeObj = new($"Edge_{i}");
            edgeObj.transform.SetParent(currentTarget.transform, false);

            var lr = edgeObj.AddComponent<LineRenderer>();
            lr.material = lineMat;
            lr.useWorldSpace = false;
            lr.positionCount = 2;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.numCapVertices = 0;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;

            lr.SetPosition(0, p[e[i, 0]]);
            lr.SetPosition(1, p[e[i, 1]]);

            edges.Add(edgeObj);
        }
    }

    public void Clear()
    {
        foreach (var go in edges)
            if (go) Destroy(go);
        edges.Clear();
        currentTarget = null;
    }
}
