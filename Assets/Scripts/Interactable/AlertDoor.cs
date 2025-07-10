using System.Collections.Generic;
using UnityEngine;

public class AlertDoor : RecordableObject
{
    public bool isOpen = true;       // ��ʼΪ��
    public bool isLocked = false;    // �رպ�����

    protected override Dictionary<string, object> CaptureExtraData()
    {
        return new Dictionary<string, object>
        {
            { "isOpen", isOpen },
            { "isLocked", isLocked }
        };
    }

    protected override void RestoreExtraData(Dictionary<string, object> data)
    {
        if (data != null)
        {
            if (data.TryGetValue("isOpen", out var v1)) isOpen = (bool)v1;
            if (data.TryGetValue("isLocked", out var v2)) isLocked = (bool)v2;

            // �����ⲿ��ʾˢ��
            var behaviour = GetComponent<AlertDoorBehaviour>();
            if (behaviour != null) behaviour.ApplyState();
        }
    }
}
