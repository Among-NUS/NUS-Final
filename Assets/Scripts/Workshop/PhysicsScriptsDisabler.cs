using UnityEngine;

public static class PhysicsScriptDisabler
{
    /// <summary>�ݹ���� Rigidbody2D/Rigidbody �����нű������</summary>
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
            if (mb is TransformLock2D) continue;   // �������������ű�
            mb.enabled = false;
        }
    }
}
