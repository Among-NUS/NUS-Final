using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D), typeof(Switch))]
public class SwitchBehaviour : MonoBehaviour, IInteractable, ICondition
{
    public UnityEvent<bool> onStateChanged = new UnityEvent<bool>();

    private InteractionManager im;
    private Switch sw;

    void Awake()
    {
        sw = GetComponent<Switch>();
        ApplyVisual();
    }

    /* ---------- IInteractable ---------- */
    public Transform GetTransform() => transform;

    public void Interact()
    {
        sw.isOn = !sw.isOn;
        onStateChanged.Invoke(sw.isOn);
        ApplyVisual();
    }

    /* ---------- 视觉反馈 ---------- */
    private void ApplyVisual()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = sw.isOn ? Color.yellow : Color.white;
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
