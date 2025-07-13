using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class PressurePlateBehaviour : MonoBehaviour, ICondition
{
    [Tooltip("哪些Tag会触发压力板")]
    public string[] validTags = { "Player", "Ghost", "Enemy" };

    [Header("贴图")]
    public Sprite idleSprite;
    public Sprite pressedSprite;

    private int triggerCount = 0;
    private SpriteRenderer sr;

    public bool IsTrue => triggerCount > 0;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ApplyVisual(); // 初始化贴图
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
