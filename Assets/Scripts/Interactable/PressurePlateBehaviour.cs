using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class PressurePlateBehaviour : MonoBehaviour, ICondition
{
    [Tooltip("��ЩTag�ᴥ��ѹ����")]
    public string[] validTags = { "Player", "Ghost", "Enemy" };

    [Header("��ͼ")]
    public Sprite idleSprite;
    public Sprite pressedSprite;

    private int triggerCount = 0;
    private SpriteRenderer sr;

    public bool IsTrue => triggerCount > 0;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ApplyVisual(); // ��ʼ����ͼ
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValid(other))
        {
            triggerCount++;
            ApplyVisual();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsValid(other))
        {
            triggerCount = Mathf.Max(triggerCount - 1, 0);
            ApplyVisual();
        }
    }

    private bool IsValid(Collider2D other)
    {
        foreach (var tag in validTags)
        {
            if (other.CompareTag(tag))
                return true;
        }
        return false;
    }

    private void ApplyVisual()
    {
        if (sr != null)
        {
            sr.sprite = triggerCount > 0 ? pressedSprite : idleSprite;
        }
    }
}
