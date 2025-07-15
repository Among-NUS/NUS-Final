using UnityEngine;

public static class WrapperColliderUtils
{
    /// <summary>
    /// 计算所有 SpriteRenderer 边界并在 Wrapper 上添加 3D BoxCollider。
    /// 不再调整 pivot，Wrapper 原点保持在合并 bounds 的几何中心。
    /// </summary>
    public static void AddBoxColliderToWrapper(GameObject wrapper, GameObject child)
    {
        // 移除旧 Collider（若有）
        if (wrapper.TryGetComponent(out BoxCollider oldCol))
            Object.Destroy(oldCol);

        Bounds combined = new Bounds();
        bool hasRenderer = false;

        foreach (var sr in child.GetComponentsInChildren<SpriteRenderer>())
        {
            if (!hasRenderer)
            {
                combined = sr.bounds;
                hasRenderer = true;
            }
            else
                combined.Encapsulate(sr.bounds);
        }

        var box = wrapper.AddComponent<BoxCollider>();

        if (hasRenderer)
        {
            Vector3 localCenter = wrapper.transform.InverseTransformPoint(combined.center);
            Vector3 localSize = wrapper.transform.InverseTransformVector(combined.size);

            localSize = new Vector3(Mathf.Abs(localSize.x),
                                    Mathf.Abs(localSize.y),
                                    Mathf.Max(0.1f, Mathf.Abs(localSize.z)));

            box.center = localCenter;
            box.size = localSize;
        }
        else
        {
            box.center = Vector3.zero;
            box.size = new Vector3(1f, 1f, 0.2f);
        }
    }
}
