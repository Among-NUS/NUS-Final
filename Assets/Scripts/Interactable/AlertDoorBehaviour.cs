using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlertDoorBehaviour : MonoBehaviour, IInteractable
{
    private InteractionManager im;
    public AlertDoor door;

    [Header("?????")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("???????")]                 // ?? ????
    public List<MonoBehaviour> conditions = new();   // ?????? ICondition ?????

    void Awake()
    {
        door = GetComponent<AlertDoor>();
        ApplyState();
    }

    void FixedUpdate()
    {

            Evaluate();                     // ????????
            ApplyState();                   // ????????

        
    }

    /* ---------- ???? ---------- */
    public Transform GetTransform() => transform;

    public void Interact()
    {
        if (!door.isOpen || door.isLocked)
        {
            Debug.Log("?????????????????????");
            return;
        }

        door.isOpen = false;
        door.isLocked = true;
        ApplyState();
        Debug.Log("??????????????");
    }

    /* ---------- ?????��? ---------- */
    void Evaluate()
    {
        if (door.isLocked) return;      // ????????????????

        // ????????????????????????????
        if (conditions.OfType<ICondition>().Any(c => c.IsTrue))
        {
            door.isOpen = false;
            door.isLocked = true;
            Debug.Log("AlertDoor ?????????????????��??");
        }
    }

    /* ---------- ??? / ?????? ---------- */
    public void ApplyState()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col) col.enabled = !door.isOpen;

        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.sprite = door.isOpen ? openSprite : closedSprite;
    }

    /* ---------- ?? InteractionManager ??? ---------- */
    void OnEnable() => im = FindObjectOfType<InteractionManager>();

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            im?.RegisterNearby(this);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            im?.UnregisterNearby(this);
    }
}
