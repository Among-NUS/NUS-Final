using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ElevatorPlatform : MonoBehaviour
{
    public enum SwitchType { AND, OR }

    [Header("条件对象")]
    public List<GameObject> conditionObjects = new();  // 拖入包含 ICondition 的物体
    public SwitchType switchType = SwitchType.OR;      // 逻辑类型

    [Header("升降设置")]
    public Transform topPosition;     // 升起位置
    public float moveSpeed = 2f;      // 升降速度

    private Vector3 startPosition;
    private Vector3 previousPosition;

    void Start()
    {
        startPosition = transform.position;
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 beforeMove = transform.position;

        EvaluateConditionsAndMove();

        Vector3 deltaMove = transform.position - beforeMove;
        MovePassengers(deltaMove);

        previousPosition = transform.position;
    }

    void EvaluateConditionsAndMove()
    {
        var conditions = conditionObjects
            .SelectMany(go => go.GetComponents<MonoBehaviour>())
            .OfType<ICondition>()
            .ToList();

        bool shouldRise = false;

        if (conditions.Count > 0)
        {
            switch (switchType)
            {
                case SwitchType.AND:
                    shouldRise = conditions.All(c => c.IsTrue);
                    break;
                case SwitchType.OR:
                    shouldRise = conditions.Any(c => c.IsTrue);
                    break;
            }
        }

        Vector3 target = shouldRise ? topPosition.position : startPosition;
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.fixedDeltaTime);
    }

    void MovePassengers(Vector3 delta)
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        Bounds bounds = col.bounds;
        Collider2D[] hits = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);

        foreach (var hit in hits)
        {
            if (hit.attachedRigidbody != null && hit.CompareTag("Player"))
            {
                hit.transform.position += delta;
            }
        }
    }
}
