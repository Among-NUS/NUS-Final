using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRecorder: MonoBehaviour
{
    [Header("auto update")]
    List<ObjectState> staticSceneRecode = new List<ObjectState>();
    List<ObjectState> dynamicSceneRecode = new List<ObjectState>();
    public ObjectState heroState = new StaticObjectState();

    public void RecordCurrentScene()
    {//ֻ���¼�̳���RecordableObject���࣬��������¼hero��ע��hero���б��лᱻ�ظ���¼
        GameObject hero = GameObject.FindWithTag("Player");
        if (hero!=null)
        {
            heroState.CaptureState(hero.GetComponent<RecordableObject>());
        }
        staticSceneRecode.Clear();
        dynamicSceneRecode.Clear();
        foreach (RecordableObject recordableObject in GameObject.FindObjectsOfType<RecordableObject>())
        {
            if (recordableObject.prefabPath==null)
            {
                StaticObjectState sos = new StaticObjectState();
                sos.CaptureState(recordableObject);
                staticSceneRecode.Add(sos);
            }
            else
            {
                DynamicObjectState dos = new DynamicObjectState();
                dos.CaptureState(recordableObject);//�˴����������屻�ݻ٣����Բ����ָ��
                dynamicSceneRecode.Add(dos);
            }
        }
    }
    public void LoadSavedScene()
    {//����س�hero֮�����������
        foreach (StaticObjectState state in staticSceneRecode)
        {
            if (state.tag != "Player") 
            {
                state.RestoreState(state.recordableObject);
            }
        }
        //ȥ�������ɵ�����
        foreach (RecordableObject recordableObject in GameObject.FindObjectsOfType<RecordableObject>())
        {
            if (recordableObject.objectState.recordableObject!=recordableObject)
            {//������δ����¼
                Debug.Log("destory " + (recordableObject.name));
                Destroy(recordableObject.gameObject);
            }
        }
        foreach (DynamicObjectState state in dynamicSceneRecode)
        {
            //�ָ���ɾ���͸ı������
            GameObject ob;
            if (state.recordableObject==null)
            {
                ob = Instantiate(Resources.Load<GameObject>(state.prefabPath));
            }
            else
            {
                ob = state.recordableObject.gameObject;
            }
            state.RestoreState(ob.GetComponent<RecordableObject>());
        }
    }
}
