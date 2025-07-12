using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��תɨ����������������ڷ������ Ray ��� Hero��
/// ��������� capturedTarget���� EnemyBehaviour ���á�
/// </summary>
public class MonitorBehaviour : MonoBehaviour
{
    /* ---------- Inspector �ɵ����� ---------- */
    [Header("auto initialize")]
    public float scanFan = 40f;   // ���νǶ�(��)
    public float rotateSpeed = 45f;   // ��ת�ٶ�(��/s) ���� Ԥ������δ��
    public float rotateRange = 180f;  // ������ת�Ƕ� ���� Ԥ������δ��
    public float scanLength = 10f;   // ���߳���
    public ScanMode sm = ScanMode.BACKANDFORTH;
    public int numberOfRay = 2;     // ���� 2 ��

    /* ---------- ����ʱ�ֶ� ---------- */
    [Header("auto update")]
    [SerializeField] List<GameObject> capturedTarget = new();
    [SerializeField] List<RaycastHit2D> hits = new();

    /* ---------- �������� ---------- */
    void Start() => numberOfRay = Mathf.Max(2, numberOfRay);

    void FixedUpdate() => CastRay();

    /* ---------- ���ģ��������߲�����Ŀ�� ---------- */
    void CastRay()
    {
        hits.Clear();
        capturedTarget.Clear();

        float sign = Mathf.Sign(transform.lossyScale.x);    // +1���ң�-1����
        float gap = scanFan / (numberOfRay - 1);
        float initAngle = transform.rotation.eulerAngles.z - scanFan * 0.5f;
        float angleRad;
        Vector3 dir;

        for (int i = 0; i < numberOfRay; ++i)
        {
            angleRad = (initAngle + gap * i) * Mathf.Deg2Rad;

            // �ھֲ��ռ䣬0�� ָ�� +X�������������ҷ���
            dir = new Vector3(Mathf.Cos(angleRad) * sign,
                              Mathf.Sin(angleRad),
                              0f);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, scanLength);
            hits.Add(hit);
            Debug.DrawLine(transform.position,
                           transform.position + dir * scanLength,
                           Color.red, 0f, false);

            if (hit.collider != null &&
                (hit.collider.GetComponent<HeroBehaviour>() != null || hit.collider.GetComponent<GhostBehaviour>() != null) &&
                !capturedTarget.Contains(hit.collider.gameObject))
            {
                capturedTarget.Add(hit.collider.gameObject);
            }
        }
    }

    /* ---------- ����ӿ� ---------- */
    public enum ScanMode { BACKANDFORTH }

    /// <summary>���ر�֡���񵽵�Ŀ���б�</summary>
    public List<GameObject> getCapturedTarget() => capturedTarget;
}
