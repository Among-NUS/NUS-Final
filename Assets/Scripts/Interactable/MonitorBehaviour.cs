using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 旋转扫描监视器：向扇形内发射多条 Ray 侦测 Hero，
/// 结果保存在 capturedTarget，供 EnemyBehaviour 调用。
/// </summary>
public class MonitorBehaviour : MonoBehaviour
{
    /* ---------- Inspector 可调参数 ---------- */
    [Header("auto initialize")]
    public float scanFan = 40f;   // 扇形角度(°)
    public float rotateSpeed = 45f;   // 旋转速度(°/s) —— 预留，暂未用
    public float rotateRange = 180f;  // 来回旋转角度 —— 预留，暂未用
    public float scanLength = 10f;   // 射线长度
    public ScanMode sm = ScanMode.BACKANDFORTH;
    public int numberOfRay = 2;     // 至少 2 条

    /* ---------- 运行时字段 ---------- */
    [Header("auto update")]
    [SerializeField] List<GameObject> capturedTarget = new();
    [SerializeField] List<RaycastHit2D> hits = new();

    /* ---------- 生命周期 ---------- */
    void Start() => numberOfRay = Mathf.Max(2, numberOfRay);

    void FixedUpdate() => CastRay();

    /* ---------- 核心：发射射线并更新目标 ---------- */
    void CastRay()
    {
        hits.Clear();
        capturedTarget.Clear();

        float sign = Mathf.Sign(transform.lossyScale.x);    // +1→右，-1→左
        float gap = scanFan / (numberOfRay - 1);
        float initAngle = transform.rotation.eulerAngles.z - scanFan * 0.5f;
        float angleRad;
        Vector3 dir;

        for (int i = 0; i < numberOfRay; ++i)
        {
            angleRad = (initAngle + gap * i) * Mathf.Deg2Rad;

            // 在局部空间，0° 指向 +X；根据缩放左右翻面
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

    /* ---------- 对外接口 ---------- */
    public enum ScanMode { BACKANDFORTH }

    /// <summary>返回本帧捕获到的目标列表。</summary>
    public List<GameObject> getCapturedTarget() => capturedTarget;
}
