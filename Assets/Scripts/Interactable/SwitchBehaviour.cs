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

    /* ---------- �Ӿ����� ---------- */
    private void ApplyVisual()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = sw.isOn ? Color.yellow : Color.white;
    }

    void Update()
    {
        ApplyVisual();  // ʵʱͬ���Ӿ�
    }


    /* ---------- ICondition ʵ�� ---------- */
    public bool IsTrue => sw.isOn;

    /* ---------- Trigger ע�� ---------- */
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
