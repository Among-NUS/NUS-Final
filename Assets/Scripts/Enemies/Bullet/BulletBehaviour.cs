using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [Header("Dynamic-object metadata")]
    public string prefabName = "Prefabs/Bullet";   // Resources 路径
    [HideInInspector] public Vector3 direction;
    public float speed = 8f;
    public float maxDistance = 5f;
    public bool isEnemy = false;

    Vector3 spawnPos;

    void Awake()
    {
        DynamicObjectManager.Instance.Register(gameObject);   // ← 注册
    }

    void OnDestroy()
    {
        if (DynamicObjectManager.Instance)
            DynamicObjectManager.Instance.Unregister(gameObject);
    }

    void Start() => spawnPos = transform.position;

    void FixedUpdate()
    {
        if (GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop) return;

        transform.position += direction.normalized * speed * Time.fixedDeltaTime;

        if (Vector3.Distance(spawnPos, transform.position) > maxDistance)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 添加阵营判断（避免友军伤害）
        if (isEnemy && collision.CompareTag("Enemy")) return;
        if (!isEnemy && (collision.CompareTag("Player") || collision.CompareTag("Ghost"))) return;
        if(collision.CompareTag("Interactable")) return;
        if (GameManager.Instance?.currentPhase == GameManager.GamePhase.TimeStop) return;

        Destroy(gameObject);
    }
}
