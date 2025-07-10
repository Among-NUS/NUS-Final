using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlertDoorBehaviour : MonoBehaviour, IInteractable
{
    private InteractionManager im;
    public AlertDoor door;

    [Header("����ͼ")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("�������")]                 // �� ����
    public List<MonoBehaviour> conditions = new();   // ��קʵ�� ICondition �����

    void Awake()
    {
        door = GetComponent<AlertDoor>();
        ApplyState();
    }

    void FixedUpdate()
    {
        Evaluate();                     // �ȼ������
        ApplyState();                   // ��ˢ����ʾ
    }

    /* ---------- ���� ---------- */
    public Transform GetTransform() => transform;

    public void Interact()
    {
        if (!door.isOpen || door.isLocked)
        {
            Debug.Log("���ѹرղ��������޷��ٿ���");
            return;
        }

        door.isOpen = false;
        door.isLocked = true;
        ApplyState();
        Debug.Log("������ҹرղ�����");
    }

    /* ---------- �����ж� ---------- */
    void Evaluate()
    {
        if (door.isLocked) return;      // �Ѿ������Ͳ����ټ��

        // ֻҪ��һ������Ϊ����������Ų�����
        if (conditions.OfType<ICondition>().Any(c => c.IsTrue))
        {
            door.isOpen = false;
            door.isLocked = true;
            Debug.Log("AlertDoor �����������������ùر�");
        }
    }

    /* ---------- �Ӿ� / ��ײˢ�� ---------- */
    public void ApplyState()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col) col.enabled = !door.isOpen;

        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.sprite = door.isOpen ? openSprite : closedSprite;
    }

    /* ---------- �� InteractionManager �Խ� ---------- */
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
