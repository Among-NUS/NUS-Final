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
        onStateChanged.Invoke(isOn);               // ֪ͨ���ж����ߣ�Door��
        GetComponent<SpriteRenderer>().color =
            isOn ? Color.yellow : Color.white;     // ��ѡ��������һ���Ӿ�����
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
}
