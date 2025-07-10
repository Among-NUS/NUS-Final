using UnityEngine;

public class TurretBehaviour : MonoBehaviour, IInteractable
{
    private InteractionManager im;
    public Turret turret;
    private Shooter shooter;
    private Monitor monitor;  // ← 添加监视器引用

    void Awake()
    {
        turret = GetComponent<Turret>();
        shooter = GetComponentInChildren<Shooter>();
        monitor = GetComponentInChildren<Monitor>();
    }

    public Transform GetTransform() => transform;

    public void Interact()
    {
        Debug.Log("Turret destroyed by interaction.");
        DestroyTurret();
    }

    public void DestroyTurret()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
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
        {
            Debug.Log("Turret dead.");
            return;
        }

        if (GameManager.Instance?.currentPhase == GameManager.GamePhase.TimeStop)
        {
            Debug.Log("Time stopped.");
            return;
        }

        if (monitor == null)
        {
            Debug.LogWarning("Monitor missing.");
            return;
        }

        int seen = monitor.getCapturedTarget().Count;

        Debug.Log(shooter);

        if (seen > 0 && shooter != null && shooter.CanFire())
        {
            Debug.Log("Turret fires!");
            shooter.Fire();
        }
    }

}
