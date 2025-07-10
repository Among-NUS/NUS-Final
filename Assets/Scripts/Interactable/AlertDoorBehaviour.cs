using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlertDoorBehaviour : MonoBehaviour, IInteractable
{
    private InteractionManager im;
    public AlertDoor door;

    [Header("门贴图")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("条件组件")]                 // ← 新增
    public List<MonoBehaviour> conditions = new();   // 拖拽实现 ICondition 的组件

    void Awake()
    {
        door = GetComponent<AlertDoor>();
        ApplyState();
    }

    void FixedUpdate()
    {
        Evaluate();                     // 先检测条件
        ApplyState();                   // 再刷新显示
    }

    /* ---------- 交互 ---------- */
    public Transform GetTransform() => transform;

    public void Interact()
    {
        if (!door.isOpen || door.isLocked)
        {
            Debug.Log("门已关闭并锁死，无法再开启");
            return;
        }

        door.isOpen = false;
        door.isLocked = true;
        ApplyState();
        Debug.Log("门由玩家关闭并锁定");
    }

    /* ---------- 条件判定 ---------- */
    void Evaluate()
    {
        if (door.isLocked) return;      // 已经锁死就不用再检查

        // 只要有一个条件为真就立即关门并锁死
        if (conditions.OfType<ICondition>().Any(c => c.IsTrue))
        {
            door.isOpen = false;
            door.isLocked = true;
            Debug.Log("AlertDoor 条件触发，门已永久关闭");
        }
    }

    /* ---------- 视觉 / 碰撞刷新 ---------- */
    public void ApplyState()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col) col.enabled = !door.isOpen;

        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.sprite = door.isOpen ? openSprite : closedSprite;
    }

    /* ---------- 与 InteractionManager 对接 ---------- */
    void OnEnable() => im = FindObjectOfType<InteractionManager>();

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
}
