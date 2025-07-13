using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Enemy))]
public class EnemyBehaviour : MonoBehaviour
{
    /* ------- Inspector 参数 ------- */
    [Header("巡逻路径(按顺序)")]
    public List<Transform> pathWayPoint;
    public PathPointMap pathPointMap;   //全图的路径点reference
    public Monitor enemyMonitor;        // 用于检测目标
    public Ear ear;                     //听觉
    public float reactionTime = 0.5f;    //敌人的反应时间
    public float interactTime = 0.2f;           //敌人需要与物体互动多久
    public float speed = 5f;
    public Animator enemyAnimator; // 用于播放动画
    public Sprite enemyDieSprite; // 死亡时的精灵



    /* ------- 运行期字段 ------- */
    private Enemy enemy;
    private Shooter shooter;          // 射击由 Shooter 负责
    private SpriteRenderer sr;
    

    /* ---------- 初始化 ---------- */
    void Awake()
    {
        enemy = GetComponent<Enemy>();
        shooter = GetComponentInChildren<Shooter>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // 根据初始缩放确定朝向
        enemy.facingLeft = transform.localScale.x < 0;
    }

    void Start()
    {
        enemy.currentTarget = 0;
        Debug.Assert(enemyAnimator != null);
        ear.onHearing += GetHearedTarget;//接受耳朵的委托，
        enemy.spotTime = Time.time;//初始化反应时间
    }

    /* ------------ 主循环 ------------ */
    void FixedUpdate()
    {   
        /* 1. 退出条件 */
        if (!enemy.isAlive) return;
        if (GameManager.Instance?.currentPhase == GameManager.GamePhase.TimeStop) return;

        if (enemy.isAlive)
        {
            enemyAnimator.SetBool("isDead", false);
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                sr.color = Color.white;
        }
        transform.localScale = new Vector3(enemy.facingLeft ? -0.5f : 0.5f, 0.5f, 0);
        PerformBehaviour();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemy.enemyState ==EnemyState.PATROL)
        {
            if (collision.gameObject == pathWayPoint[enemy.currentTarget].gameObject)
            {
                enemy.currentTarget = (enemy.currentTarget + 1) % pathWayPoint.Count;
            }
        }
        if (enemy.enemyState == EnemyState.CHASE)
        {
            if (enemy.curChasePoint >= enemy.chasePath.Count)
            {//当前正在前往最后目标
                return;
            }
            if (collision.gameObject == pathPointMap.pathPointObjects[GetNormalIndex(enemy.chasePath[enemy.curChasePoint])])//注意chase两兄弟都是序号
            {
                enemy.curChasePoint++;
                Debug.Log("moving to " + enemy.curChasePoint);
            }
        }


        BulletBehaviour bullet = collision.GetComponent<BulletBehaviour>();

        // 如果碰到的是来自玩家的子弹
        if (bullet != null && !bullet.isEnemy && enemy.isAlive)
        {
            Debug.Log("Enemy hit by player bullet");
            DestroyEnemy();  // 敌人死亡处理
        }
    }
    void PerformBehaviour()
    {
        switch (enemy.enemyState) 
        { 
            case EnemyState.PATROL:
                Patrol();
                break;
            case EnemyState.DISCOVER:
                Discover();
                break;
            case EnemyState.CHASE:
                Chase();
                break;
            case EnemyState.SHOOT:
                Shoot();
                break;
            case EnemyState.DEAD:
                break;
        }

    }
    void Patrol()
    {
        /* 巡逻移动 */
        Vector3 dirMove = Vector3.zero;
        if (pathWayPoint != null && pathWayPoint.Count > 1)
        {
            dirMove = (pathWayPoint[enemy.currentTarget].position - transform.position).normalized;
            transform.position += dirMove * speed * Time.fixedDeltaTime;
            enemy.facingLeft = dirMove.x < 0;
        }
        else if(pathWayPoint.Count == 1)
        {
            //空白填充
            enemy.currentTarget = 0;
        }
            
        if (enemyMonitor.getCapturedTarget().Count!=0)
        {
            enemy.spotTime = Time.time;//初始化发现时间
            enemy.lastTarget = GetClosestPathPoint(enemyMonitor.getCapturedTarget()[0]);//固定最后发现的位置
            enemy.lastPriciseTarget = enemyMonitor.getCapturedTarget()[0].transform.position;
            enemy.enemyState = EnemyState.DISCOVER;
        }
    }
    void Discover()
    {
        if (enemyMonitor.getCapturedTarget().Count == 0)
        {
            enemy.enemyState =EnemyState.CHASE;//消失时拿着上一帧的位置进chase
            enemy.isLastTargetUpdated = true;
            return;
        }
        else
        {
            enemy.lastTarget = GetClosestPathPoint(enemyMonitor.getCapturedTarget()[0]);//固定最后发现的位置
            enemy.lastPriciseTarget = enemyMonitor.getCapturedTarget()[0].transform.position;
        }
        if ((Time.time- enemy.spotTime)>reactionTime)
        {
            enemy.enemyState = EnemyState.SHOOT;
        }
    }
    void Shoot()
    {
        if (enemyMonitor.getCapturedTarget().Count == 0)
        {
            enemy.enemyState = EnemyState.CHASE;//消失时拿着上一帧的位置进chase
            enemy.isLastTargetUpdated = true;
        }
        else
        {
            enemy.lastTarget = GetClosestPathPoint(enemyMonitor.getCapturedTarget()[0]);//固定最后发现的位置
            enemy.lastPriciseTarget = enemyMonitor.getCapturedTarget()[0].transform.position;
            Vector3 dirShoot = enemyMonitor.getCapturedTarget()[0].transform.position - transform.position;
            enemy.facingLeft = dirShoot.x < 0;       // 始终面向目标
            shooter.faceLeft = enemy.facingLeft;     // 告诉 Shooter 发射方向

            if (shooter.CanFire())
                shooter.Fire();
        }
    }
    void Chase()
    {
        if (enemy.isLastTargetUpdated)
        {//如果更新了，那就刷新路径
            enemy.chasePath = BFS(GetClosestPathPoint(gameObject), enemy.lastTarget);
            enemy.curChasePoint = 0;//注意这里不会维护自身在哪个节点，只会认为自己在最近的点，特定路径点排列会引发bug
            enemy.isLastTargetUpdated = false;
        }
        //按路径行走
        if (enemyMonitor.getCapturedTarget().Count != 0)
        {//先判定是否检测到主角
            enemy.lastTarget = GetClosestPathPoint(enemyMonitor.getCapturedTarget()[0]);//固定最后发现的位置
            enemy.lastPriciseTarget = enemyMonitor.getCapturedTarget()[0].transform.position;
            enemy.enemyState = EnemyState.DISCOVER;
            enemy.isGoBack = false;
        }
        if (enemy.curChasePoint <= enemy.chasePath.Count)
        {//路径还没有走完
            Vector3 dirMove = Vector3.zero;
            if (enemy.chasePath != null && enemy.chasePath.Count > 0&& enemy.curChasePoint != enemy.chasePath.Count)
            {//未达到最后一个点
                if (enemy.chasePath[enemy.curChasePoint] < 50)
                {
                    dirMove = (pathPointMap.pathPointObjects[enemy.chasePath[enemy.curChasePoint]].transform.position - transform.position).normalized;
                    transform.position += speed * Time.fixedDeltaTime * dirMove;
                    enemy.facingLeft = dirMove.x < 0;
                }
                else
                {
                    if (enemy.timeProcessingElevator ==-1f)
                    {//未开始计时，则开始计时
                        enemy.timeProcessingElevator = Time.time;
                    }
                    if ((Time.time - enemy.timeProcessingElevator) > interactTime)
                    {//已开始计时，则继续计时
                        transform.position = pathPointMap.pathPointObjects[enemy.chasePath[enemy.curChasePoint] - 50].transform.position;//识别为电梯，直接传送:p
                        enemy.timeProcessingElevator = -1f;
                    }
                }

            }
            else
            {//达到最后一个点
                if ((enemy.lastPriciseTarget - transform.position).magnitude<0.1f)
                {
                    enemy.curChasePoint++;
                }
                dirMove =  (enemy.lastPriciseTarget - transform.position).normalized;
                transform.position += speed * Time.fixedDeltaTime * dirMove;
                enemy.facingLeft = dirMove.x < 0;
            }
            
        }
        else
        {//路径走完了
            if (enemy.isGoBack)
            {//是在回去的路上
                enemy.enemyState = EnemyState.PATROL;
                enemy.currentTarget = (enemy.currentTarget + 1) % pathWayPoint.Count;//已到达该巡逻路径，前往下一个
                enemy.isGoBack = false;
            }
            else
            {//开始向回走
                enemy.isLastTargetUpdated = true;
                enemy.lastTarget = pathPointMap.pathPointObjects.LastIndexOf(pathWayPoint[enemy.currentTarget].gameObject);
                enemy.lastPriciseTarget = pathPointMap.pathPointObjects[enemy.lastTarget].transform.position;
                enemy.isGoBack = true;
            }
        }
    }
    List<int> BFS(int start,int target)
    {//返回整型数组，代表路径，同时包括是否需要坐电梯
        Dictionary<int,int> cameFrom = new Dictionary<int, int> ();
        bool[] isChecked = new bool[pathPointMap.pathPointObjects.Count];
        MyGraph graph = pathPointMap.graph;
        Queue<Node> toCheck = new Queue<Node>();
        toCheck.Enqueue(graph.nodes[start]);
        while (toCheck.Count > 0)
        {
            Node cur = toCheck.Dequeue();
            for (int i = 0; i < cur.neightbor.Length; i++)
            {
                if (cur.neightbor[i] == -1)
                {
                    continue;
                }
                if (isChecked[GetNormalIndex(cur.neightbor[i])])
                {
                    continue;
                }
                cameFrom.Add(cur.neightbor[i], cur.index);//key是边的方向和种类，value这是该边由这个点发出
                toCheck.Enqueue(graph.nodes[GetNormalIndex(cur.neightbor[i])]);
                isChecked[cur.index] = true;
                if (graph.nodes[GetNormalIndex(cur.neightbor[i])].index == target)
                {
                    goto outerLoop;
                }
            }
        }
        outerLoop:
        List<int> res = new List<int>();
        int curTarget = target;
        while (cameFrom.ContainsKey(curTarget) | cameFrom.ContainsKey(curTarget + 50))
        {
            bool isElevator = cameFrom.ContainsKey(curTarget + 50);
            res.Add(isElevator? curTarget + 50: curTarget);//这里记录的是key，
            curTarget = cameFrom[isElevator ? curTarget + 50 : curTarget];
        }
        res.Reverse();
        return res;
    }
    int GetNormalIndex(int x)
    {//因为边中需要做电梯的对应neightbor值会+50，需要特别处理
        return x >= 50 ? x - 50 : x;
    }
    int GetClosestPathPoint(GameObject target)
    {//获取地图上距离目标最近的点
        int closest = 0;
        float distance = Mathf.Infinity;
        if (pathPointMap.pathPointObjects.Count==0)
        {
            Debug.LogError("This is pathPointMap, where are my pathPoints?");
        }
        for (int i = 0;i<pathPointMap.pathPointObjects.Count;i++)
        {
            float curDis = (pathPointMap.pathPointObjects[i].transform.position - target.transform.position).magnitude;
            if (distance > curDis)
            {
                closest = i;
                distance = curDis;
            }
        }
        return closest;
    }
    void GetHearedTarget(Collider2D other)
    {//耳朵委托的实现
        Debug.Log("I heared!");
        if (enemy.enemyState ==EnemyState.PATROL)
        {
            enemy.enemyState =EnemyState.CHASE;
        }
        if (enemy.enemyState == EnemyState.CHASE)
        {
            int newTarget = GetClosestPathPoint(other.gameObject);
            enemy.lastTarget = newTarget;//声音仅在巡逻和追寻时更新目标
            enemy.lastPriciseTarget = other.gameObject.transform.position;
            enemy.isLastTargetUpdated = true;//该状态只能在chase中消除
            enemy.isGoBack = false;
        }
    }
    public enum EnemyState 
    { 
        PATROL,
        DISCOVER,
        CHASE,
        SHOOT,
        DEAD
    }
    public void DestroyEnemy()
    {
        enemyAnimator.SetBool("isDead", true);
        if (!enemy.isAlive) return;
        enemy.enemyState = EnemyState.DEAD;
        enemy.isAlive = false;
    }

}
