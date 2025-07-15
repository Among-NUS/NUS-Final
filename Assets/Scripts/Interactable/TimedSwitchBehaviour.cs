using System.Numerics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D), typeof(TimedSwitch), typeof(SpriteRenderer))]
public class TimedSwitchBehaviour : MonoBehaviour, IInteractable, ICondition
{
    public UnityEvent<bool> onStateChanged = new UnityEvent<bool>();

    [Header("Switch Sprites")]
    public Sprite offSprite;
    public Sprite onSprite;

    private InteractionManager im;
    private TimedSwitch tsw;
    private SpriteRenderer sr;
    public float switchCooldown = 1.5f;
    public float setCooldown = 1.5f;
    public Transform square;
    private SpriteRenderer squareSR;

    void Awake()
    {
        tsw = GetComponent<TimedSwitch>();
        sr = GetComponent<SpriteRenderer>();
        squareSR = square.GetComponent<SpriteRenderer>();
        ApplyVisual();
    }

    /* ---------- IInteractable ---------- */
    public Transform GetTransform() => transform;

    public void Interact()
    {
        if (GameManager.Instance.currentPhase == GameManager.GamePhase.TimeStop) return;
        if (!tsw.isOn)
        {
            tsw.isOn = !tsw.isOn;
            tsw.switchCooldown = setCooldown;
            onStateChanged.Invoke(tsw.isOn);
            ApplyVisual();
        }
        
    }

    /* ---------- �Ӿ����� ---------- */
    private void ApplyVisual()
    {
        if (sr != null)
            sr.sprite = tsw.isOn ? onSprite : offSprite;

        if (tsw.isOn)
        {
            squareSR.enabled = true;
            square.transform.localScale = new UnityEngine.Vector3(tsw.switchCooldown / setCooldown, square.transform.localScale.y, square.transform.localScale.z);
        }
        else
        {
            squareSR.enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (tsw.isOn && tsw.switchCooldown > 0f)
        {
            tsw.switchCooldown -= Time.deltaTime;
        }
        if (tsw.switchCooldown < 0f)
        {
            tsw.switchCooldown = 0f;
            tsw.isOn = false;
            onStateChanged.Invoke(tsw.isOn);
            tsw.switchCooldown = setCooldown;
        }
        ApplyVisual();  // ʵʱͬ���Ӿ�
    }

    /* ---------- ICondition ʵ�� ---------- */
    public bool IsTrue => tsw.isOn;

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
