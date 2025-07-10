using UnityEngine;
using System.Collections.Generic;

public class RecordableObject : MonoBehaviour
{//凡是需记录的物体都继承这个类
    [Header("need update!")]
    public string prefabPath = null;//是否为预制体，可能被摧毁和创建
    public ObjectState objectState;//是否有对应的记录

    public virtual Dictionary<string, object> CaptureExtraData()
    {
        return null; // 默认无额外数据
    }

    public virtual void RestoreExtraData(Dictionary<string, object> data)
    {
        // 默认无操作
    }
}
