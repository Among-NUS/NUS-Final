using System.Collections.Generic;
using UnityEngine;
using static EnemyBehaviour;

/// <summary>
/// �����Լ��Ŀɼ�¼���ݣ��ṹ��ȫ�հ� Turret.
/// </summary>
public class Enemy : RecordableObject
{
    public bool isAlive = true;
    public int lastTarget;              //��õ�Ӧ��ǰ����λ��,��currentTargetû�й�ϵ-------
    public Vector3 lastPriciseTarget;   //���Ӧ��ǰ���ľ�ȷλ��----------
    public bool isLastTargetUpdated = false; //�ж�Ӧ��ǰ����λ���Ƿ����-------------
    public float timeProcessingElevator = -1f;   //���������忪ʼ������ʱ��--------------
    public float spotTime;               //�������ǵ�ʱ�䣬���ڷ�Ӧ--------------------
    public List<int> chasePath;         //׷��·��---------------------
    public int curChasePoint;           //��¼׷���ĸ�λ��,���---------------------
    public int currentTarget;           //Ѳ��λ�ü�¼------------------
    public bool isGoBack = false;       //���˴�׷���λ�ûص�Ѳ�ߵ�λ��---------------------
    public bool facingLeft = true;   // �� ��ǰ����---------------------
    public EnemyState enemyState;     //enemy��ǰ״̬---------------------

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
