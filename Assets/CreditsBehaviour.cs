using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsBehaviour : MonoBehaviour
{
    public RectTransform creditsPanel;
    public float speed = 50f;
    public float endOffset = 300f;
    private bool isRolling = false;
    private Vector3 startPos;
    private float panelHeight;

    // Start is called before the first frame update
    void Start()
    {
        startPos = creditsPanel.localPosition;

        // 获取内容高度
        panelHeight = creditsPanel.GetComponent<RectTransform>().rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRolling) return;

        // 每帧往上移动
        creditsPanel.localPosition += Vector3.up * speed * Time.deltaTime;

        // 当 Credits 滚出一定距离就停止
        if (creditsPanel.localPosition.y >= startPos.y + panelHeight + endOffset)
        {
            isRolling = false;
            Debug.Log("Credits 滚动结束");
        }
    }
    
    public void StartCredits()
    {
        // 重置位置并开始滚动
        creditsPanel.localPosition = startPos;
        isRolling = true;
    }
}
