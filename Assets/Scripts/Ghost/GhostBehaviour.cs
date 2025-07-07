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
            transform.position += DirFromKey(key) * speed;
        }
        if (records.Count == 0) Destroy(gameObject);
    }

    Vector3 DirFromKey(char key) => key switch
    {
        'a' => Vector3.left,
        'd' => Vector3.right,
        'w' => Vector3.up,
        's' => Vector3.down,
        _ => Vector3.zero
    };
}
