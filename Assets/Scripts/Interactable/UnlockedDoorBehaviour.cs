using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class UnlockedDoorBehaviour : MonoBehaviour
{
    [Header("设置")]
    public List<string> triggerTags = new List<string> { "Player", "Ghost", "Enemy" };

    [Header("状态")]
    public bool isOpen = false;

    private BoxCollider2D box;
    private SpriteRenderer sr;

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
            Debug.Log("门已打开");
        }
        else if (count == 0 && isOpen)
        {
            isOpen = false;
            ApplyState();
            Debug.Log("门已关闭");
        }
    }

    void ApplyState()
    {
        sr.color = isOpen ? Color.green : Color.red;
    }
}
