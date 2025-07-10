using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Enemy))]
public class EnemyBehaviour : MonoBehaviour
{
    /* ------- Inspector 参数 ------- */
    [Header("巡逻路径(按顺序)")]
    public List<Transform> pathWayPoint;
    public Monitor enemyMonitor;      // 用于检测目标
    public float speed = 5f;

    /* ------- 运行期字段 ------- */
    private int currentTarget;
    private bool facingLeft = true;   // ← 当前朝向
    private Enemy enemy;
    private Shooter shooter;          // 射击由 Shooter 负责
    private SpriteRenderer sr;

    /* ---------- 初始化 ---------- */
    void Awake()
    {
        enemy = GetComponent<Enemy>();
        shooter = GetComponentInChildren<Shooter>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // 根据初始缩放确定朝向
        facingLeft = transform.localScale.x < 0;
    }

    void Start() => currentTarget = 0;

    /* ------------ 主循环 ------------ */
    void FixedUpdate()
    {
        /* 1. 退出条件 */
        if (!enemy.isAlive) return;
        if (GameManager.Instance?.currentPhase == GameManager.GamePhase.TimeStop) return;

        if (enemy.isAlive)
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                sr.color = Color.white;
        }

        /* 2. 巡逻移动 */
        Vector3 dirMove = Vector3.zero;
        if (pathWayPoint != null && pathWayPoint.Count > 0)
        {
            dirMove = (pathWayPoint[currentTarget].position - transform.position).normalized;
            transform.position += dirMove * speed * Time.fixedDeltaTime;
        }

        /* 3. 侦测 & 射击 */
        var targets = enemyMonitor != null ? enemyMonitor.getCapturedTarget() : null;
        if (targets != null && targets.Count > 0)
        {
            Vector3 dirShoot = targets[0].transform.position - transform.position;
            facingLeft = dirShoot.x < 0;       // 始终面向目标
            shooter.faceLeft = facingLeft;           // 告诉 Shooter 发射方向

            if (shooter.CanFire())
                shooter.Fire();
        }
        else if (dirMove.x != 0)                     // 没看到目标 ⇒ 按移动方向翻转
        {
            facingLeft = dirMove.x < 0;
        }

        /* 4. 左右镜像翻转 —— 与 Hero 同一做法 */
        Vector3 s = transform.localScale;
        float targetSign = facingLeft ? -1f : 1f;    // prefab 默认朝右
        if (Mathf.Sign(s.x) != targetSign)
        {
            s.x = Mathf.Abs(s.x) * targetSign;
            transform.localScale = s;
        }
    }

    /* ---------- 巡逻点切换 ---------- */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pathWayPoint != null &&
            collision.gameObject == pathWayPoint[currentTarget].gameObject)
        {
            currentTarget = (currentTarget + 1) % pathWayPoint.Count;
        }

        BulletBehaviour bullet = collision.GetComponent<BulletBehaviour>();

        // 如果碰到的是来自玩家的子弹
        if (bullet != null && !bullet.isEnemy && enemy.isAlive)
        {
            Debug.Log("Enemy hit by player bullet");
            DestroyEnemy();  // 敌人死亡处理
        }
    }

    public void DestroyEnemy()
    {
        if (!enemy.isAlive) return;

        enemy.isAlive = false;
        if (sr != null) sr.color = Color.red;
    }

}
