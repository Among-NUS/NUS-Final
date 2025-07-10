using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable]
public class ObjectState {//将获取到的可记录物体的信息记录下来并独立存在
    public RecordableObject recordableObject;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Dictionary<string, object> extraData;
    public ObjectState() {}
    public virtual void CaptureState(RecordableObject recordableObject)
    {   
        this.recordableObject = recordableObject;
        position = recordableObject.transform.position;
        rotation = recordableObject.transform.rotation;
        scale = recordableObject.transform.localScale;
        extraData = recordableObject.CaptureExtraData();
        recordableObject.objectState = this;//回传指针
    }

    public void RestoreState(RecordableObject ro)
    {
        GameObject gameObject = ro.gameObject;
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        gameObject.transform.localScale = scale;
        ro.RestoreExtraData(this.extraData);
    }
}

public class StaticObjectState: ObjectState
{
    public int id;//gameobject的id
    public string tag;//该物体的tag，用于特殊处理
    public override void CaptureState(RecordableObject recordableObject)
    {
        base.CaptureState(recordableObject);
        this.id = recordableObject.GetInstanceID();
        this.tag = recordableObject.gameObject.tag;
    }
}

[System.Serializable]
public class DynamicObjectState : ObjectState
{//原本挂在在物体上的脚本可能被摧毁，需单独创建实例保存
    public string prefabPath;      // Resources.Load 路径//只有预制体可以被创建和摧毁
    public override void CaptureState(RecordableObject recordableObject)
    {
        base.CaptureState(recordableObject);
        this.prefabPath = recordableObject.prefabPath;
    }
}

