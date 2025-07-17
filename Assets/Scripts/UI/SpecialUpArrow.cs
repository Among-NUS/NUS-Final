using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialUpArrow : MonoBehaviour
{
    public float moveSpeed = 2f;   // 振动速度
    public float moveRange = 1f;   // 振动幅度
    private Vector3 startPosition; // 初始位置

    public bool isUp = true;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // 计算沿自身up方向的偏移
        Vector3 direction = isUp ? transform.up : transform.right;
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveRange;
        transform.position = startPosition + direction * offset;
    }
}

