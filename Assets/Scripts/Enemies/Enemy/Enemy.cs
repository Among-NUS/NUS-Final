using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人自己的可记录数据，结构完全照搬 Turret.
/// </summary>
public class Enemy : RecordableObject
{
    public bool isAlive = true;

    protected override Dictionary<string, object> CaptureExtraData()
    {
        return new Dictionary<string, object>
        {
            { "isAlive", isAlive }
        };
    }

    protected override void RestoreExtraData(Dictionary<string, object> data)
    {
        if (data != null && data.TryGetValue("isAlive", out var v))
            isAlive = (bool)v;
    }
}
