using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D), typeof(Switch), typeof(SpriteRenderer))]
public class SwitchBehaviour : MonoBehaviour, IInteractable, ICondition
{
    public UnityEvent<bool> onStateChanged = new UnityEvent<bool>();

    [Header("Switch Sprites")]
    public Sprite offSprite;
    public Sprite onSprite;

    private InteractionManager im;
    private Switch sw;
    private SpriteRenderer sr;

    void Awake()
    {
        sw = GetComponent<Switch>();
        sr = GetComponent<SpriteRenderer>();
        ApplyVisual();
    }

    /* ---------- IInteractable ---------- */
    public Transform GetTransform() => transform;

    public void Interact()
    {
        if (GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop) return;
        sw.isOn = !sw.isOn;
        onStateChanged.Invoke(sw.isOn);
        ApplyVisual();
    }

    /* ---------- 视觉反馈 ---------- */
    private void ApplyVisual()
    {
        if (sr != null)
            sr.sprite = sw.isOn ? onSprite : offSprite;
    }

    void Update()
    {
        ApplyVisual();  // 实时同步视觉
    }

    /* ---------- ICondition 实现 ---------- */
    public bool IsTrue => sw.isOn;

    /* ---------- Trigger 注册 ---------- */
    void OnEnable() => im = FindObjectOfType<InteractionManager>();

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) im?.RegisterNearby(this);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player")) im?.UnregisterNearby(this);
    }
}
