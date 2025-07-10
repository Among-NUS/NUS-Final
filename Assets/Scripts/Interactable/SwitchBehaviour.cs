using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class SwitchBehaviour : MonoBehaviour, IInteractable, ICondition  // �� ���� ICondition
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
        onStateChanged.Invoke(isOn);               // ֪ͨ���ж����ߣ����ţ�
        GetComponent<SpriteRenderer>().color =
            isOn ? Color.yellow : Color.white;     // ��ѡ�Ӿ�����
    }

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

    /* ---------- ��¶ֻ������ ---------- */
    public bool IsOn => isOn;

    /* ---------- ICondition ʵ�� ---------- */
    public bool IsTrue => isOn;
}
