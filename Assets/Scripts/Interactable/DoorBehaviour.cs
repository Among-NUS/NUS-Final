using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Door))]
public class DoorBehaviour : MonoBehaviour
{
    [Header("?????")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("????????")]
    public List<GameObject> conditionObjects = new();  // ??????????? ICondition ?? GameObject
    public SwitchType st = SwitchType.OR;

    private Door door;
    private BoxCollider2D box;

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        door = GetComponent<Door>();
    }

    void Start()
    {
        Evaluate();
    }

    void FixedUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(box.bounds.center, box.bounds.size, 0f);
        int count = 0;

        foreach (var hit in hits)
        {
            if (hit.tag=="Enemy" && hit.gameObject != gameObject)
            {
                count++;
            }
        }

        if (count > 0 && !door.isOpen)
        {
            door.isOpen = true;
            ApplyState();
            Debug.Log("门已打开");
            return;
        }
        else if (count == 0 && door.isOpen)
        {
            door.isOpen = false;
            ApplyState();
            Debug.Log("门已关闭");
            return;
        }
        Evaluate();
    }

    void Evaluate()
    {
        bool prev = door.isOpen;

        // ??????? conditionObjects ????? ICondition ?????
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
