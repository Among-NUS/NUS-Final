using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;               // ���� Transform
    public float smoothSpeed = 2f;
    public float horizontalOffset = 2f;
    public float fixedZ = -10f;
    public float verticalOffset = 2.5f;

    private bool facingLeft = false;       // ���ǳ���
    private Component heroScript;          // ���� Hero �ű�

    void Start()
    {
        if (target != null)
            heroScript = target.GetComponent<MonoBehaviour>(); // �� MonoBehaviour �������нű�
    }

    void FixedUpdate()
    {
        if (target == null || heroScript == null)
            return;

        // ����ͨ�������ȡ hero �� public bool facingLeft
        var type = heroScript.GetType();
        var field = type.GetField("facingLeft");
        if (field != null)
        {
            object val = field.GetValue(heroScript);
            if (val is bool b) facingLeft = b;
        }

        // ƫ��λ��
        float offsetX = facingLeft ? -horizontalOffset : horizontalOffset;
        Vector3 offset = new Vector3(offsetX, 0f, 0f);

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + verticalOffset, fixedZ) + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
