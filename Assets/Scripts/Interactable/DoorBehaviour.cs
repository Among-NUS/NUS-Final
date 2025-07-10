using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    [Header("门贴图")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("条件组件")]
    public List<MonoBehaviour> conditions = new();  // 可拖拽任何实现 ICondition 的组件
    public SwitchType st = SwitchType.OR;

    private bool isOpen;

    void Awake()
    {
        Evaluate(); // 初始自检
    }

    void Update()
    {
        Evaluate(); // 每帧检测条件是否变化（也可以按需改成事件驱动）
    }

    void Evaluate()
    {
        bool prev = isOpen;

        // 统一处理所有 ICondition（Switch 也实现了它）
        var validConditions = conditions.OfType<ICondition>().ToList();
        if (validConditions.Count == 0) return;

        switch (st)
        {
            case SwitchType.AND:
                isOpen = validConditions.All(c => c.IsTrue);
                break;
            case SwitchType.OR:
                isOpen = validConditions.Any(c => c.IsTrue);
                break;
            case SwitchType.CHANGE:
                isOpen = !isOpen;
                break;
        }

        if (prev != isOpen)
            ApplyState();
    }


    void ApplyState()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col) col.enabled = !isOpen;

        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.sprite = isOpen ? openSprite : closedSprite;
    }

    public enum SwitchType { AND, OR, CHANGE }
}
