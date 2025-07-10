using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [Header("Dynamic-object metadata")]
    public string prefabName = "Prefabs/Bullet";   // Resources 路径
    [HideInInspector] public Vector3 direction;
    public float speed = 8f;
    public float maxDistance = 5f;

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

    void OnTriggerEnter2D(Collider2D _) => Destroy(gameObject);
}
