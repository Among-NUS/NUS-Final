using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class RewindBehaviour : MonoBehaviour
{
    public float speed = 0.1f;
    public float recordDuration = 5f;

    Queue<char> _inputs = new();        // 本体始终往这里写
    bool _isReplaying = false;          // 区分录制 / 回放
    float _startTime;

    // ---------- Unity 生命周期 ----------
    void Start() => _startTime = Time.time;

    void Update()
    {
        // 到点生成克隆体
        if (Time.time - _startTime > recordDuration && !_isReplaying)
        {
            SpawnClone();
            _startTime = float.MaxValue;         // 只生成一次
        }

        // 录制 or 回放
        if (!_isReplaying) RecordInput();
        else PlayBack();
    }

    // ---------- 录制 ----------
    void RecordInput()
    {
        if (Input.GetKey(KeyCode.A)) Write('a', Vector3.left);
        else if (Input.GetKey(KeyCode.D)) Write('d', Vector3.right);
        else if (Input.GetKey(KeyCode.W)) Write('w', Vector3.up);
        else if (Input.GetKey(KeyCode.S)) Write('s', Vector3.down);
    }

    void Write(char key, Vector3 dir)
    {
        _inputs.Enqueue(key);
        transform.position += dir * speed;
    }

    // ---------- 回放 ----------
    void PlayBack()
    {
        if (_inputs.Count == 0) {  return; }

        char key = _inputs.Dequeue();
        Vector3 dir = key switch
        {
            'a' => Vector3.left,
            'd' => Vector3.right,
            'w' => Vector3.up,
            's' => Vector3.down,
            _ => Vector3.zero
        };
        transform.position += dir * speed;
    }

    // ---------- 克隆 ----------
    void SpawnClone()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/RewindObject"), new Vector3(0f, 0f, 0f), Quaternion.identity);

        // 给克隆体一份“录像带”并开启回放
        var clone = obj.GetComponent<RewindBehaviour>();
        clone.StartReplay(new Queue<char>(_inputs));   // 深拷贝！

        // 你可以选择清空原队列，或继续累积新的操作
        _inputs.Clear();
    }

    // 供外部调用的初始化
    public void StartReplay(Queue<char> recordedInputs)
    {
        _inputs = recordedInputs;
        _isReplaying = true;
    }
}
