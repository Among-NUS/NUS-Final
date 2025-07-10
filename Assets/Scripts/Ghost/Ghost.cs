using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class Ghost : MonoBehaviour
{//模仿时现身，不模仿隐身，一直存在于场景中,一定有刚体和碰撞箱
    [Header("need to update")]
    
    [Header("auto update")]
    public bool isHurt = false;//暂时方案，判定受伤使用ontrigger回调
    public KeyRecorder kr;//本组件和keyrecorder装载同一对象上
    public int keySquenceIndex = 0;

    public bool isTouchGround = false;//提供给模仿方法，某些特定行为需要条件
    public bool isInteracting = false;//向可互动物体传递是否在互动

    public delegate void ghostImitation(Rigidbody2D myRb, List<KeyRecorder.OperationType> opt);//需要hero来实现如何模仿hero的动作
    public static ghostImitation Imitation;
    Rigidbody2D rb;

    public bool isReplay = false;
    bool lastIsPlay = true;
    // Start is called before the first frame update
    void Start()
    {
        kr = GetComponent<KeyRecorder>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        if (kr==null)
        {
            Debug.LogError("Ghost 没有记录键盘的组件！");
        }
        if (rb==null)
        {
            Debug.LogError("Ghost 没有刚体！");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lastIsPlay^isReplay)
        {//检测变化沿
            GetComponent<Collider2D>().enabled = isReplay;
            GetComponent<SpriteRenderer>().enabled = isReplay;
            rb.gravityScale = 0;//防止下落，重力在模仿时会恢复
            rb.velocity = Vector3.zero;//防止漂移

        }
        lastIsPlay = isReplay;
        if (isReplay)
        {
            if (Imitation != null)
            {
                if (keySquenceIndex < kr.keyboardSequence.Count)
                {
                    Imitation(rb,kr.keyboardSequence[keySquenceIndex]);
                    keySquenceIndex++;
                }
                else
                {
                    keySquenceIndex = 0;
                    isReplay = false;
                }
            }
            else
            {
                Debug.LogError("Ghost don't know how to move!");
            }
        }
    }
}
