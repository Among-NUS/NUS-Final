using UnityEngine;

public class Bullet2D : MonoBehaviour
{
    public float speed = 8f;
    public float maxDistance = 5f;

    private Vector2 startPosition;

    [HideInInspector] public Vector2 direction = Vector2.down; // 外部设置的发射方向

    void Start()
    {
        // 记录子弹的初始位置
        startPosition = transform.position;

        // 归一化方向，避免在 Update() 中每帧都归一化（效率更高）
        direction = direction.normalized;
    }

    void Update()
{
    if (this == null) return;  // 防止后续访问销毁对象

    transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

    float distanceTraveled = Vector2.Distance(startPosition, transform.position);
    if (distanceTraveled > maxDistance)
    {
        Destroy(gameObject);
        return;  // 立即返回，避免后续访问
    }
}


}
