using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Door))]
public class DoorBehaviour : MonoBehaviour
{
    [Header("门贴图")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("条件对象")]
    public List<GameObject> conditionObjects = new();  // 拖入任意包含 ICondition 的 GameObject
    public SwitchType st = SwitchType.OR;

    private Door door;

    void Awake()
    {
        door = GetComponent<Door>();
    }

    void Start()
    {
        Evaluate();
    }

    void FixedUpdate()
    {
        Evaluate();
    }

    void Evaluate()
    {
        bool prev = door.isOpen;

        // 获取所有 conditionObjects 中实现 ICondition 的组件
        var validConditions = conditionObjects
            .SelectMany(go => go.GetComponents<MonoBehaviour>())
            .OfType<ICondition>()
            .ToList();

        if (validConditions.Count == 0) return;

        switch (st)
        {
            case SwitchType.AND:
                door.isOpen = validConditions.All(c => c.IsTrue);
                break;
            case SwitchType.OR:
                door.isOpen = validConditions.Any(c => c.IsTrue);
                break;
            case SwitchType.CHANGE:
                door.isOpen = !door.isOpen;
                break;
        }

        ApplyState();
    }

    void ApplyState()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col) col.enabled = !door.isOpen;

        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.sprite = door.isOpen ? openSprite : closedSprite;
    }

    public enum SwitchType { AND, OR, CHANGE }
}
