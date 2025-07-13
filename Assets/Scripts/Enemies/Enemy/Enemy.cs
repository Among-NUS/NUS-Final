using System.Collections.Generic;
using UnityEngine;
using static EnemyBehaviour;

/// <summary>
/// 敌人自己的可记录数据，结构完全照搬 Turret.
/// </summary>
public class Enemy : RecordableObject
{
    public bool isAlive = true;
    public int lastTarget;              //获得的应该前往的位置,和currentTarget没有关系-------
    public Vector3 lastPriciseTarget;   //获得应该前往的精确位置----------
    public bool isLastTargetUpdated = false; //判断应该前往的位置是否更新-------------
    public float timeProcessingElevator = -1f;   //敌人与物体开始互动的时间--------------
    public float spotTime;               //发现主角的时间，用于反应--------------------
    public List<int> chasePath;         //追逐路径---------------------
    public int curChasePoint;           //记录追逐到哪个位置,序号---------------------
    public int currentTarget;           //巡逻位置记录------------------
    public bool isGoBack = false;       //敌人从追逐的位置回到巡逻的位置---------------------
    public bool facingLeft = true;   // ← 当前朝向---------------------
    public EnemyState enemyState;     //enemy当前状态---------------------

    protected override Dictionary<string, object> CaptureExtraData()
    {
        return new Dictionary<string, object>
        {
            { "isAlive", isAlive },
            { "lastTarget", lastTarget },
            { "lastPriciseTarget", lastPriciseTarget },
            { "isLastTargetUpdated", isLastTargetUpdated },
            { "timeProcessingElevator", timeProcessingElevator },
            { "spotTime", spotTime },
            { "chasePath", chasePath },
            { "curChasePoint", curChasePoint },
            { "isGoBack", isGoBack },
            { "facingLeft", facingLeft },
            { "enemyState", enemyState },
            { "currentTarget", currentTarget }
        };
    }

    protected override void RestoreExtraData(Dictionary<string, object> data)
    {
        if (data != null && data.TryGetValue("isAlive", out var v))
            isAlive = (bool)v;
        if (data.TryGetValue("lastTarget", out var lastTarget))
            this.lastTarget = (int)lastTarget;
        if (data.TryGetValue("lastPriciseTarget", out var lastPriciseTarget))
            this.lastPriciseTarget = (Vector3)lastPriciseTarget;
        if (data.TryGetValue("isLastTargetUpdated", out var isLastTargetUpdated))
            this.isLastTargetUpdated = (bool)isLastTargetUpdated;
        if (data.TryGetValue("timeProcessingElevator", out var timeProcessingElevator))
            this.timeProcessingElevator = (float)timeProcessingElevator;
        if (data.TryGetValue("spotTime", out var spotTime))
            this.spotTime = (float)spotTime;
        if (data.TryGetValue("chasePath", out var chasePath))
            this.chasePath = (List<int>)chasePath;
        if (data.TryGetValue("curChasePoint", out var curChasePoint))
            this.curChasePoint = (int)curChasePoint;
        if (data.TryGetValue("isGoBack", out var isGoBack))
            this.isGoBack = (bool)isGoBack;
        if (data.TryGetValue("facingLeft", out var facingLeft))
            this.facingLeft = (bool)facingLeft;
        if (data.TryGetValue("enemyState", out var enemyState))
            this.enemyState = (EnemyState)enemyState;
        if (data.TryGetValue("currentTarget", out var currentTarget))
            this.currentTarget = (int)currentTarget;
    }
}
