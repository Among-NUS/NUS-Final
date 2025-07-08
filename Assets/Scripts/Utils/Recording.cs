using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Snapshot
{
    public Vector3 playerPosition;
    public List<ObjectState> objectStates;

    // �չ��캯�������ٰ��� Unity API ����
    public Snapshot() { }

    // ��ʽ��ʼ���������ں��ʵ�ʱ������ Start������
    public void Capture()
    {
        GameObject hero = GameObject.FindWithTag("Player");
        playerPosition = hero ? hero.transform.position : Vector3.zero;

        objectStates = new List<ObjectState>();
        foreach (var obj in GameObject.FindObjectsOfType<RecordableObject>())
            objectStates.Add(obj.CaptureState());

        DynamicObjectManager.Instance.Capture();
    }

    public void Restore()
    {
        foreach (var state in objectStates)
        {
            if (state.id == "Player") continue;

            var obj = RecordableObject.FindByID(state.id);
            if (obj != null)
            {
                obj.RestoreState(state);
            }
        }

        DynamicObjectManager.Instance.Restore();
    }
}




public class Record
{
    public List<char> keys;

    public Record(List<char> keysThisFrame)
    {
        keys = keysThisFrame;
    }
}

