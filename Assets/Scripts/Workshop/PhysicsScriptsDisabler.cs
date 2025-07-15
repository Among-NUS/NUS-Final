using UnityEngine;

public static class PhysicsScriptDisabler
{
    /// <summary>递归禁用 Rigidbody2D/Rigidbody 和所有脚本组件。</summary>
    public static void Disable(GameObject root)
    {
        foreach (var rb2d in root.GetComponentsInChildren<Rigidbody2D>(true))
        {
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            rb2d.simulated = false;
        }

        foreach (var rb in root.GetComponentsInChildren<Rigidbody>(true))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        foreach (var mb in root.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (mb is TransformLock2D) continue;   // 保留网格锁定脚本
            mb.enabled = false;
        }
    }
}
