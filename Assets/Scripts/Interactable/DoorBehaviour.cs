using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    [Header("�Ͻ���")]
    [SerializeField] private Sprite closedSprite;   // door_close
    [SerializeField] private Sprite openSprite;     // door_open
    
    [Header("need to drag")]
    public List<Switch> switches = new();          // ֱ���Ͻű�����
    public SwitchType st = SwitchType.OR;

    bool isOpen;

    void Awake()
    {
        // �������п����¼�
        foreach (var sw in switches)
            sw.onStateChanged.AddListener(_ => Evaluate());

        Evaluate();                                // ��ʼ�Լ�
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
                isOpen = !isOpen;                  // ��һ���ط�ת������״̬
                break;
        }

        if (prev != isOpen) ApplyState();
    }

    void ApplyState()
    {
        // true == ���ţ�������ײ�� + ��ɫ
        var col = GetComponent<BoxCollider2D>();
        if (col) col.enabled = !isOpen;
        //change image not color
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.sprite = isOpen ? openSprite : closedSprite;
    }

    public enum SwitchType { AND, OR, CHANGE }
}
