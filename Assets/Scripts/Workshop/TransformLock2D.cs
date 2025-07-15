using UnityEngine;

public class TransformLock2D : MonoBehaviour
{
    public float gridSize = 1f;

    private Vector3 lastPosition;
    private Vector3 originalScale;
    private Transform targetChild; // 实际显示的子物体

    void Awake()
    {
        // 找 Prefab 子物体
        if (transform.childCount > 0)
            targetChild = transform.GetChild(0);

        // 记录原始缩放（优先用子物体缩放，没有就用自身）
        if (targetChild != null)
            originalScale = targetChild.localScale;
        else
            originalScale = transform.localScale;
    }

    void LateUpdate()
    {
        // 锁定缩放
        if (targetChild != null)
        {
            if (targetChild.localScale != originalScale)
                targetChild.localScale = originalScale;
        }
        else
        {
            if (transform.localScale != originalScale)
                transform.localScale = originalScale;
        }

        // 判断位置变化
        if ((transform.position - lastPosition).sqrMagnitude < 0.0001f)
            return;

        lastPosition = transform.position;
    }

    void OnMouseUp()
    {
        // 鼠标释放吸附网格
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
        pos.y = Mathf.Round(pos.y / gridSize) * gridSize;
        pos.z = 1f;
        transform.position = pos;

        // 再锁定缩放
        if (targetChild != null)
        {
            if (targetChild.localScale != originalScale)
                targetChild.localScale = originalScale;
        }
        else
        {
            if (transform.localScale != originalScale)
                transform.localScale = originalScale;
        }
    }
}
