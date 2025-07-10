using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : RecordableObject
{//���ش�������������tagΪplayer
    [Header("need to drag")]

    [Header("auto initiate")]
    public float horizontalAcc = 5f;
    public float jumpImpulse = 20f;
    public float maxVerticalSpeed = 30f;
    public float maxHorizontalSpeed = 20f;
    public float maxGrabWallFallingSpeed = -0.5f;
    public float grabWallForceScale = 1f;
    public float wallJumpHorizontalForceScale = 0.3f;
    public float normalGravity = 1.2f;
    public float shootInterval = 0.5f;

    [Header("auto update")]
    public bool isTouchGround = false;
    public bool isTouchLeft = false;
    public bool isTouchRight = false;
    public bool isTouchTop = false;
    public int move;
    public bool isGrabbingWall = false;

    public float lastShoot;//��¼��һ�ο����ʱ��

    public bool isInteracting = false;//��ɻ������崫���Ƿ��ڻ���

    public bool isHurt = false;

    Rigidbody2D selfRb;
    [Header("float error")]
    public float EPSILON = 1e-5f;
    // Start is called before the first frame update
    void Start()
    {
        selfRb = GetComponent<Rigidbody2D>();
        selfRb.freezeRotation = true;
        selfRb.gravityScale = normalGravity;
        Ghost.Imitation += GhostMove;
        lastShoot = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GhostMove(selfRb,KeyRecorder.CollectKeyInputs());
    }

    void GhostMove(Rigidbody2D myRb,List<KeyRecorder.OperationType> opt)
    {//���ڸ�����ƶ�
        bool isGhost = (myRb != selfRb);
        bool isToughGroundMoving = isTouchGround;
        float lastShootMoving = lastShoot;
        if (isGhost)
        {//�����ghost��ͬ������������������Ϊghost��isTouchGround��lastShoot
            myRb.mass = selfRb.mass;
            myRb.gravityScale = selfRb.gravityScale;
            myRb.freezeRotation = true;
            isToughGroundMoving = myRb.GetComponent<Ghost>().isTouchGround;
            lastShootMoving = myRb.GetComponent<Ghost>().lastShoot;
            myRb.GetComponent<Ghost>().isInteracting = opt.Contains(KeyRecorder.OperationType.INTERACTE);
        }
        else
        {
            myRb.GetComponent<Hero>().isInteracting = opt.Contains(KeyRecorder.OperationType.INTERACTE);
        }
            //�����ƶ�
            //��ȡ����
        move = opt.Contains(KeyRecorder.OperationType.LEFT) ^ opt.Contains(KeyRecorder.OperationType.RIGHT) ? 1 : 0;
        move = opt.Contains(KeyRecorder.OperationType.LEFT) ? -move : move;

        if (Mathf.Abs(myRb.velocity.x) <= maxHorizontalSpeed)
        {//ȷ��������
            myRb.AddForce(new Vector2(move * horizontalAcc, 0));
            if (move!=0) 
            { 
                myRb.transform.right = move*Vector2.right;
            }//���ﳯ��ǰ������
        }
        else
        {//ǿ������
            myRb.velocity = new Vector2(Mathf.Sign(myRb.velocity.x) * maxHorizontalSpeed, myRb.velocity.y);
        }
        //�����ƶ���,�����ƶ�����Ͱ��������෴,�ٶȿ��ټ���
        if (move == 0|(move*myRb.velocity.x)<0)
        {
            myRb.velocity = new Vector2(myRb.velocity.x / 1.2f, myRb.velocity.y);
        }


        //��Ծ
        if (opt.Contains(KeyRecorder.OperationType.UP))
        {//���ո�������,�Ӵ������ǽ�ڲſ�����Ծ
            //ƽ����Ծ
            if (isToughGroundMoving)
            {
                myRb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
            }
            //��ǽ��Ծ
            //else if ((isTouchLeft | isTouchRight) && !isTouchGround)
            //{
            //    myRb.AddForce(
            //        new Vector2((isTouchLeft ? 1 : -1) * wallJumpHorizontalForceScale * jumpImpulse, jumpImpulse),
            //        ForceMode2D.Impulse);
            //}
        }

        //�����ٶ�����
        if (myRb.velocity.y < -maxVerticalSpeed)
        {//ȷ�������ٶȲ�����,����Ϸ������ֻ���������䵼��
            myRb.velocity = new Vector2(myRb.velocity.x, -maxVerticalSpeed);
        }

        //��ǽ�����ٶȼ���,��������ٶ�
        //������ǽ��δ�Ӵ����棬���¶�Ӧ��������������ٶ�Ϊ��������������ǽ����
        //isGrabbingWall =
        //    ((isTouchLeft && move == -1) | (isTouchRight && move == 1)) && (myRb.velocity.y < 0 && myRb.velocity.y < -EPSILON) && !isTouchGround;
        //if (isGrabbingWall)
        //{//ǽ��Ħ���ʹ�ֱ�ٶȳ�����,����������
        //    myRb.AddForce(
        //        new Vector2(0, -(myRb.velocity.y - maxGrabWallFallingSpeed) * grabWallForceScale)
        //        );
        //}
        if(opt.Contains(KeyRecorder.OperationType.SHOOT))
        {//�������һ����һ��
            if (Time.time-lastShoot>shootInterval)
            {
                GameObject bullet =  Instantiate(
                    Resources.Load<GameObject>("Prefabs/Bullet"),
                    myRb.transform.position,
                    Quaternion.Euler(0,0,myRb.transform.right.x*-90)//�ӵ���tranform.up�����ж�
                    );
                bullet.GetComponent<Bullets>().isFriendly = true;
            }
        }
    }
    public override Dictionary<string, object> CaptureExtraData()
    {
        return new Dictionary<string, object>() {
            { "velocity",selfRb.velocity}
        };
    }//ֻ��Ҫ���棬����Ҫ�ָ�
}
