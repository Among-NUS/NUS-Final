using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ??????????????????????????? Ray ??? Hero??
/// ????????? capturedTarget???? EnemyBehaviour ???¨¢?
/// </summary>
public class MonitorBehaviour : MonoBehaviour
{
    /* ---------- Inspector ??????? ---------- */
    [Header("auto initialize")]
    public float scanFan = 40f;   // ???¦Í??(??)
    public float rotateSpeed = 45f;   // ??????(??/s) ???? ???????¦Ä??
    public float rotateRange = 180f;  // ?????????? ???? ???????¦Ä??
    public float scanLength = 10f;   // ???????
    public ScanMode sm = ScanMode.BACKANDFORTH;
    public int numberOfRay = 2;     // ???? 2 ??

    /* ---------- ???????? ---------- */
    [Header("auto update")]
    [SerializeField] List<GameObject> capturedTarget = new();
    [SerializeField] List<RaycastHit2D> hits = new();

    /* ---------- ???????? ---------- */
    void Start() => numberOfRay = Mathf.Max(2, numberOfRay);

    void FixedUpdate() => CastRay();

    /* ---------- ????????????????????? ---------- */
    void CastRay()
    {
        hits.Clear();
        capturedTarget.Clear();

        float sign = Mathf.Sign(transform.lossyScale.x);    // +1?????-1????
        float gap = scanFan / (numberOfRay - 1);
        float initAngle = transform.rotation.eulerAngles.z - scanFan * 0.5f;
        float angleRad;
        Vector3 dir;

        for (int i = 0; i < numberOfRay; ++i)
        {
            angleRad = (initAngle + gap * i) * Mathf.Deg2Rad;

            // ???????0?? ??? +X?????????????????
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

    /* ---------- ?????? ---------- */
    public enum ScanMode { BACKANDFORTH }

    /// <summary>???????????????§Ò???</summary>
    public List<GameObject> getCapturedTarget() => capturedTarget;
}
