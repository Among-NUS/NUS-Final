using UnityEngine;

public class TransformLock2D : MonoBehaviour
{
    public float gridSize = 1f;

    private Vector3 lastPosition;
    private Vector3 originalScale;
    private Transform targetChild; // ʵ����ʾ��������

    void Awake()
    {
        // �� Prefab ������
        if (transform.childCount > 0)
            targetChild = transform.GetChild(0);

        // ��¼ԭʼ���ţ����������������ţ�û�о�������
        if (targetChild != null)
            originalScale = targetChild.localScale;
        else
            originalScale = transform.localScale;
    }

    void LateUpdate()
    {
        // ��������
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

        // �ж�λ�ñ仯
        if ((transform.position - lastPosition).sqrMagnitude < 0.0001f)
            return;

        lastPosition = transform.position;
    }

    void OnMouseUp()
    {
        // ����ͷ���������
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
        pos.y = Mathf.Round(pos.y / gridSize) * gridSize;
        pos.z = 1f;
        transform.position = pos;

        // ����������
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
