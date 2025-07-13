using UnityEngine;

public class TurretBehaviour : MonoBehaviour, IInteractable
{
    private InteractionManager im;
    public Turret turret;
    private Shooter shooter;
    private MonitorBehaviour monitor;  // ← 添加监视器引用
    public Sprite aliveSprite;
    public Sprite dieSprite;
    public float reactionTime = 0.5f;
    private float spotTime = 0f;//not in recordable
    private bool playerSpotted = false;
    public Animator turretAnimator;


    void Awake()
    {
        turret = GetComponent<Turret>();
        shooter = GetComponentInChildren<Shooter>();
        monitor = GetComponentInChildren<MonitorBehaviour>();
        turretAnimator = GetComponentInChildren<Animator>();
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
            if (!playerSpotted)
            {
                playerSpotted = true;
                spotTime = Time.time;
            }
            else
            {
                if (Time.time - spotTime > reactionTime)
                {
                    GameObject target = targets[0];
                    Vector3 dir = target.transform.position - transform.position;
                    shooter.faceLeft = dir.x < 0;

                    if (shooter.CanFire())
                    {
                        turretAnimator.SetBool("isShooting",true);
                        shooter.Fire();
                    }
                }

            }
            // 获取最近目标位置判断左右

        }
        else
        {
            turretAnimator.SetBool("isShooting",false);
            playerSpotted = false;
        }
    }


}
