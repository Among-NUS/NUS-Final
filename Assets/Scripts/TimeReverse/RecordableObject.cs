using UnityEngine;
using System.Collections.Generic;

public class RecordableObject : MonoBehaviour
{//�������¼�����嶼�̳������
    [Header("need update!")]
    public string prefabPath = null;//�Ƿ�ΪԤ���壬���ܱ��ݻٺʹ���
    public ObjectState objectState;//�Ƿ��ж�Ӧ�ļ�¼

    public virtual Dictionary<string, object> CaptureExtraData()
    {
        return null; // Ĭ���޶�������
    }

    public virtual void RestoreExtraData(Dictionary<string, object> data)
    {
        // Ĭ���޲���
    }
}
