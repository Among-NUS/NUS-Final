using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : RecordableObject
{//搭载此组件的物体必须tag为player
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

    public float lastShoot;//记录上一次开火的时间

    public bool isInteracting = false;//向可互动物体传递是否在互动

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
    {//基于刚体的移动
        bool isGhost = (myRb != selfRb);
        bool isToughGroundMoving = isTouchGround;
        float lastShootMoving = lastShoot;
        if (isGhost)
        {//如果是ghost，同步质量和重力，更换为ghost的isTouchGround和lastShoot
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
            //左右移动
            //获取输入
        move = opt.Contains(KeyRecorder.OperationType.LEFT) ^ opt.Contains(KeyRecorder.OperationType.RIGHT) ? 1 : 0;
        move = opt.Contains(KeyRecorder.OperationType.LEFT) ? -move : move;

        if (Mathf.Abs(myRb.velocity.x) <= maxHorizontalSpeed)
        {//确保不超速
            myRb.AddForce(new Vector2(move * horizontalAcc, 0));
            if (move!=0) 
            { 
                myRb.transform.right = move*Vector2.right;
            }//人物朝向前进方向
        }
        else
        {//强制限速
            myRb.velocity = new Vector2(Mathf.Sign(myRb.velocity.x) * maxHorizontalSpeed, myRb.velocity.y);
        }
        //不按移动键,或者移动方向和按键方向相反,速度快速减少
        if (move == 0|(move*myRb.velocity.x)<0)
        {
            myRb.velocity = new Vector2(myRb.velocity.x / 1.2f, myRb.velocity.y);
        }


        //跳跃
        if (opt.Contains(KeyRecorder.OperationType.UP))
        {//检测空格上升沿,接触地面或墙壁才可以跳跃
            //平地跳跃
            if (isToughGroundMoving)
            {
                myRb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
            }
            //蹬墙跳跃
            //else if ((isTouchLeft | isTouchRight) && !isTouchGround)
            //{
            //    myRb.AddForce(
            //        new Vector2((isTouchLeft ? 1 : -1) * wallJumpHorizontalForceScale * jumpImpulse, jumpImpulse),
            //        ForceMode2D.Impulse);
            //}
        }

        //下落速度限制
        if (myRb.velocity.y < -maxVerticalSpeed)
        {//确保下落速度不超速,本游戏场景中只可能由下落导致
            myRb.velocity = new Vector2(myRb.velocity.x, -maxVerticalSpeed);
        }

        //贴墙下落速度减少,结合下落速度
        //左右贴墙，未接触地面，按下对应方向键，且下落速度为正数，则正在贴墙下落
        //isGrabbingWall =
        //    ((isTouchLeft && move == -1) | (isTouchRight && move == 1)) && (myRb.velocity.y < 0 && myRb.velocity.y < -EPSILON) && !isTouchGround;
        //if (isGrabbingWall)
        //{//墙面摩擦和垂直速度成正比,该项受重力
        //    myRb.AddForce(
        //        new Vector2(0, -(myRb.velocity.y - maxGrabWallFallingSpeed) * grabWallForceScale)
        //        );
        //}
        if(opt.Contains(KeyRecorder.OperationType.SHOOT))
        {//射击，按一次射一次
            if (Time.time-lastShoot>shootInterval)
            {
                GameObject bullet =  Instantiate(
                    Resources.Load<GameObject>("Prefabs/Bullet"),
                    myRb.transform.position,
                    Quaternion.Euler(0,0,myRb.transform.right.x*-90)//子弹向tranform.up方向行动
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
    }//只需要保存，不需要恢复
}
