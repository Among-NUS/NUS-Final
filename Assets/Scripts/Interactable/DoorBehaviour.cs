using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Door))]
public class DoorBehaviour : MonoBehaviour
{
    [Header("����ͼ")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("�������")]
    public List<MonoBehaviour> conditions = new();  // ����ק�κ�ʵ�� ICondition �����
    public SwitchType st = SwitchType.OR;

    private Door door;

    void Awake()
    {
        door = GetComponent<Door>();
    }

    void Start()
    {
        Evaluate(); // ��ʼ�Լ�
    }

    void FixedUpdate()
    {
        Evaluate(); // ÿ֡��������Ƿ�仯��Ҳ���԰���ĳ��¼�������
    }

    void Evaluate()
    {
        bool prev = door.isOpen;

        // ͳһ�������� ICondition��Switch Ҳʵ��������
        var validConditions = conditions.OfType<ICondition>().ToList();
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
