using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    [Header("����ͼ")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("�������")]
    public List<MonoBehaviour> conditions = new();  // ����ק�κ�ʵ�� ICondition �����
    public SwitchType st = SwitchType.OR;

    private bool isOpen;

    void Awake()
    {
        Evaluate(); // ��ʼ�Լ�
    }

    void Update()
    {
        Evaluate(); // ÿ֡��������Ƿ�仯��Ҳ���԰���ĳ��¼�������
    }

    void Evaluate()
    {
        bool prev = isOpen;

        // ͳһ�������� ICondition��Switch Ҳʵ��������
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
