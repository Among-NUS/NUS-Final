using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField]
    public bool isOn;
    public UnityEvent<bool> onStateChanged = new UnityEvent<bool>();

    InteractionManager im;

    /* ---------- IInteractable ---------- */
    public Transform GetTransform() => transform;

    public void Interact()
    {
        isOn = !isOn;
        onStateChanged.Invoke(isOn);               // 通知所有订阅者（Door）
        GetComponent<SpriteRenderer>().color =
            isOn ? Color.yellow : Color.white;     // 可选：给开关一个视觉反馈
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
}
