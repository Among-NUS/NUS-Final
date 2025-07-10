using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class Ghost : MonoBehaviour
{//ģ��ʱ������ģ������һֱ�����ڳ�����,һ���и������ײ��
    [Header("need to update")]
    
    [Header("auto update")]
    public bool isHurt = false;//��ʱ�������ж�����ʹ��ontrigger�ص�
    public KeyRecorder kr;//�������keyrecorderװ��ͬһ������
    public int keySquenceIndex = 0;

    public bool isTouchGround = false;//�ṩ��ģ�·�����ĳЩ�ض���Ϊ��Ҫ����
    public bool isInteracting = false;//��ɻ������崫���Ƿ��ڻ���

    public delegate void ghostImitation(Rigidbody2D myRb, List<KeyRecorder.OperationType> opt);//��Ҫhero��ʵ�����ģ��hero�Ķ���
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
            Debug.LogError("Ghost û�м�¼���̵������");
        }
        if (rb==null)
        {
            Debug.LogError("Ghost û�и��壡");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lastIsPlay^isReplay)
        {//���仯��
            GetComponent<Collider2D>().enabled = isReplay;
            GetComponent<SpriteRenderer>().enabled = isReplay;
            rb.gravityScale = 0;//��ֹ���䣬������ģ��ʱ��ָ�
            rb.velocity = Vector3.zero;//��ֹƯ��

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
