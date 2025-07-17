using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;               // 主角 Transform
    public float smoothSpeed = 2f;
    public float horizontalOffset = 2f;
    public float fixedZ = -10f;
    public float verticalOffset = 2.5f;

    private bool facingLeft = false;       // 主角朝向
    private Component heroScript;          // 缓存 Hero 脚本

    void Start()
    {
        if (target != null)
            heroScript = target.GetComponent<MonoBehaviour>(); // 用 MonoBehaviour 兼容所有脚本
    }

    void FixedUpdate()
    {
        if (target == null || heroScript == null)
            return;

        // 尝试通过反射读取 hero 的 public bool facingLeft
        var type = heroScript.GetType();
        var field = type.GetField("facingLeft");
        if (field != null)
        {
            object val = field.GetValue(heroScript);
            if (val is bool b) facingLeft = b;
        }

        // 偏移位置
        float offsetX = facingLeft ? -horizontalOffset : horizontalOffset;
        Vector3 offset = new Vector3(offsetX, 0f, 0f);

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + verticalOffset, fixedZ) + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
