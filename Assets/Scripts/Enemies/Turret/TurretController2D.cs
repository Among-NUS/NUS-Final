using UnityEngine;

public class TurretController2D : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public Transform target;
    public bool playerInside = false;
    public float fireInterval = 0.5f;
    public float detectDistance = 8f;
    public float detectAngle = 45f;
    public float interactDistance = 5f;

    private float fireCooldown = 0f;  // 冷却时间计时器

    public Turret turret;          // ← 只有这一份状态
    void Awake() => turret = GetComponent<Turret>();

    void FixedUpdate()
    {
        if (GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop) return;

        if (turret.isAlive)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white; // 炮塔存活时显示正常颜色
        }
        else
        {
            return; // 如果炮塔已被销毁，则不执行后续逻辑
        }

        // 玩家按 E 且靠近时销毁炮塔
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Turret destroyed by player interaction.");
            DestroyTurret();
            return;
        }

        // 更新冷却时间
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.fixedDeltaTime;
        }

        // 检测并发射
        bool inSight = TargetInSight();
        if (inSight && fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = fireInterval; // 重置冷却时间
        }
    }

    void Fire()
    {
        if (target == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = (target.position - firePoint.position).normalized;
        bullet.GetComponent<Bullet2D>().direction = dir;
    }

    bool TargetInSight()
    {
        if (target == null)
            return false;

        Vector2 dirToTarget = target.position - transform.position;
        float distance = dirToTarget.magnitude;

        if (distance > detectDistance)
            return false;

        float angleToTarget = Vector2.Angle(Vector2.left, dirToTarget);
        if (angleToTarget > detectAngle)
            return false;

        return true;
    }

    // 判断玩家是否足够靠近以进行交互（比如靠近炮塔 1.5f 范围内）
    bool CanInteractWithTarget()
    {
        if (target == null)
            return false;

        float distance = Vector2.Distance(transform.position, target.position);
        return distance <= interactDistance;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    public void DestroyTurret()
    {
        fireCooldown = 0f; // 清除冷却时间
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        turret.isAlive = false; // 设置炮塔为不可用状态
    }
}