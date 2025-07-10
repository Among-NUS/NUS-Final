using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable]
public class ObjectState {//����ȡ���Ŀɼ�¼�������Ϣ��¼��������������
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
        recordableObject.objectState = this;//�ش�ָ��
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
    public int id;//gameobject��id
    public string tag;//�������tag���������⴦��
    public override void CaptureState(RecordableObject recordableObject)
    {
        base.CaptureState(recordableObject);
        this.id = recordableObject.GetInstanceID();
        this.tag = recordableObject.gameObject.tag;
    }
}

[System.Serializable]
public class DynamicObjectState : ObjectState
{//ԭ�������������ϵĽű����ܱ��ݻ٣��赥������ʵ������
    public string prefabPath;      // Resources.Load ·��//ֻ��Ԥ������Ա������ʹݻ�
    public override void CaptureState(RecordableObject recordableObject)
    {
        base.CaptureState(recordableObject);
        this.prefabPath = recordableObject.prefabPath;
    }
}

