using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class UnlockedDoorBehaviour : MonoBehaviour
{
    [Header("����")]
    public List<string> triggerTags = new List<string> { "Player", "Ghost", "Enemy" };
    public Sprite openDoorSprite;
    public Sprite closedDoorSprite;

    [Header("״̬")]
    public bool isOpen = false;

    private BoxCollider2D box;
    private SpriteRenderer sr;
    private int IgnoreRaycast = 2;
    private int Default = 0;

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        ApplyState();
    }

    void FixedUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(box.bounds.center, box.bounds.size, 0f);
        int count = 0;

        foreach (var hit in hits)
        {
            if (triggerTags.Contains(hit.tag) && hit.gameObject != gameObject)
            {
                count++;
            }
        }

        if (count > 0 && !isOpen)
        {
            isOpen = true;
            ApplyState();
            Debug.Log("���Ѵ�");
        }
        else if (count == 0 && isOpen)
        {
            isOpen = false;
            ApplyState();
            Debug.Log("���ѹر�");
        }
    }

    void ApplyState()
    {
        sr.sprite = isOpen ? openDoorSprite : closedDoorSprite;
        transform.gameObject.layer = isOpen ? IgnoreRaycast : Default;
    }
}
