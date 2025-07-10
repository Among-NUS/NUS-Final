using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����Լ��Ŀɼ�¼���ݣ��ṹ��ȫ�հ� Turret.
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
