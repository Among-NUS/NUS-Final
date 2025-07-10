using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class SwitchBehaviour : MonoBehaviour, IInteractable, ICondition  // ← 加上 ICondition
{
    [SerializeField]
    public bool isOn;
    public UnityEvent<bool> onStateChanged = new UnityEvent<bool>();

    private InteractionManager im;

    /* ---------- IInteractable ---------- */
    public Transform GetTransform() => transform;

    public void Interact()
    {
        isOn = !isOn;
        onStateChanged.Invoke(isOn);               // 通知所有订阅者（如门）
        GetComponent<SpriteRenderer>().color =
            isOn ? Color.yellow : Color.white;     // 可选视觉反馈
    }

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

    /* ---------- 暴露只读属性 ---------- */
    public bool IsOn => isOn;

    /* ---------- ICondition 实现 ---------- */
    public bool IsTrue => isOn;
}
