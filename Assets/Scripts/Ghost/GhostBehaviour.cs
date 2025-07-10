using UnityEngine;
using System.Collections.Generic;

public class GhostBehaviour : MonoBehaviour
{
    public float speed = 0.1f;
    Queue<Record> records;
    bool isReplaying = false;

    public void StartReplay(Queue<Record> inputRecords)
    {
        records = inputRecords;
        isReplaying = true;
    }

    void FixedUpdate()
    {
        if (!isReplaying || records == null || records.Count == 0) return;

        Record frame = records.Dequeue();
        foreach (char key in frame.keys)
        {
            if (key == 'w' || key == 's')
                TryUseStairs(key == 'w');
            if (key == 'e')
                TryDestroyTurret();

            transform.position += DirFromKey(key) * speed;
        }
        if (records.Count == 0)
        {
            Destroy(gameObject);
            GameManager.Instance.OnGhostFinished();
        }
    }

    Vector3 DirFromKey(char key) => key switch
    {
        'a' => Vector3.left,
        'd' => Vector3.right,
        //'w' => Vector3.up,
        //'s' => Vector3.down,
        _ => Vector3.zero
    };
    void TryUseStairs(bool commandUp)
    {
        // 用极小半径采样当前位置可能重叠到的楼梯触发器
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.05f);
        foreach (var h in hits)
        {
            StairsBehaviour stairs = h.GetComponent<StairsBehaviour>();
            if (stairs != null)
            {
                stairs.TryTeleport(transform, commandUp);
                break;                      // 一次只找最近一段楼梯即可
            }
        }
    }
    void TryDestroyTurret()
    {
        // 用极小半径采样当前位置可能重叠到的炮塔触发器
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.05f);
        foreach (var h in hits)
        {
            TurretBehaviour turret = h.GetComponent<TurretBehaviour>();
            if (turret != null && turret.turret.isAlive)
            {
                turret.DestroyTurret();
                break;                      // 一次只找最近一座炮塔即可
            }
        }
    }
}
