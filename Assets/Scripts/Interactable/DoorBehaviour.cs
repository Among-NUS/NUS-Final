using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    [Header("need to drag")]
    public List<Switch> switches = new();          // 直接拖脚本引用
    public SwitchType st = SwitchType.OR;

    bool isOpen;

    void Awake()
    {
        // 订阅所有开关事件
        foreach (var sw in switches)
            sw.onStateChanged.AddListener(_ => Evaluate());

        Evaluate();                                // 初始自检
    }

    void Evaluate()
    {
        bool prev = isOpen;

        switch (st)
        {
            case SwitchType.AND:
                isOpen = switches.All(s => s.IsOn);
                break;
            case SwitchType.OR:
                isOpen = switches.Any(s => s.IsOn);
                break;
            case SwitchType.CHANGE:
                isOpen = !isOpen;                  // 任一开关翻转就切门状态
                break;
        }

        if (prev != isOpen) ApplyState();
    }

    void ApplyState()
    {
        // true == 开门：禁用碰撞器 + 改色
        var col = GetComponent<BoxCollider2D>();
        if (col) col.enabled = !isOpen;

        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = isOpen ? Color.red : Color.green;
    }

    public enum SwitchType { AND, OR, CHANGE }
}
