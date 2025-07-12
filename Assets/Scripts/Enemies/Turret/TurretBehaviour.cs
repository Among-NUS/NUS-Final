using UnityEngine;

public class TurretBehaviour : MonoBehaviour, IInteractable
{
    private InteractionManager im;
    public Turret turret;
    private Shooter shooter;

    private Monitor monitor;  // ← 添加监视器引用
    public Sprite aliveSprite;
    public Sprite dieSprite;


    void Awake()
    {
        turret = GetComponent<Turret>();
        shooter = GetComponentInChildren<Shooter>();
        monitor = GetComponentInChildren<MonitorBehaviour>();
    }

    public Transform GetTransform() => transform;

    public void Interact()
    {
        Debug.Log("Turret destroyed by interaction.");
        DestroyTurret();
    }

    public void DestroyTurret()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = dieSprite;
        turret.isAlive = false;
    }

    void OnEnable()
    {
        im = FindObjectOfType<InteractionManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            im?.RegisterNearby(this);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            im?.UnregisterNearby(this);
    }

    void FixedUpdate()
    {
        if (!turret.isAlive)
            return;

        if (GameManager.Instance?.currentPhase == GameManager.GamePhase.TimeStop)
            return;

        if (monitor == null || shooter == null)
            return;

        if (turret.isAlive)
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                sr.sprite = aliveSprite;
        }

        var targets = monitor.getCapturedTarget();
        int seen = targets.Count;

        if (seen > 0)
        {
            // 获取最近目标位置判断左右
            GameObject target = targets[0];
            Vector3 dir = target.transform.position - transform.position;
            shooter.faceLeft = dir.x < 0;

            if (shooter.CanFire())
            {
                shooter.Fire();
            }
        }
    }


}
