using Unity.VisualScripting;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [Header("Dynamic-object metadata")]
    public string prefabName = "Prefabs/Bullet";   // Resources 路径
    [HideInInspector] public Vector3 direction;
    public float speed = 20f;
    public float maxDistance = 5f;
    public bool isEnemy = false;
    public LineRenderer lineRenderer;

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

    void Start()
    {
        spawnPos = transform.position;
        speed = 70f;//强制改变速度
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }
    

    void FixedUpdate()
    {
        if (GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, speed * Time.fixedDeltaTime,LayerMask.GetMask("Enemy","Hero","Default"));
        if (hit.collider!=null&&IsHit(hit.collider))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position += direction.normalized * speed * Time.fixedDeltaTime;
        }
        lineRenderer.SetPosition(0, transform.position- direction.normalized * speed * Time.fixedDeltaTime);
        lineRenderer.SetPosition(1, transform.position);
        if (Vector3.Distance(spawnPos, transform.position) > maxDistance)
            Destroy(gameObject);
    }
    bool IsHit(Collider2D collision)
    {
        // 添加阵营判断（避免友军伤害）
        if (isEnemy && collision.CompareTag("Enemy")) return false;
        if (!isEnemy && (collision.CompareTag("Player") || collision.CompareTag("Ghost"))) return false;
        if (collision.CompareTag("Interactable")) return false;
        if (GameManager.Instance?.currentPhase == GameManager.GamePhase.TimeStop) return false;
        if (collision.GetComponent<Enemy>() != null && collision.GetComponent<Enemy>().isAlive == false) return false;
        return true;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsHit(collision))
        {
            return;
        }

        //在击中不同目标时产生不同的效果
        if ((collision.CompareTag("Player") || collision.CompareTag("Ghost"))|| collision.CompareTag("Enemy"))
        {//击中人物
            Instantiate(Resources.Load<GameObject>("Prefabs/hitEnemyEffectPrefab"),transform.position,transform.rotation);
        }else
        {//击中其他
            Instantiate(Resources.Load<GameObject>("Prefabs/hitWallEffectPrefab"), transform.position, transform.rotation);
        }
            Destroy(gameObject);
    }
}
