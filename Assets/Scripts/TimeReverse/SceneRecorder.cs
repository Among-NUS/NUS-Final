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
    {//只会记录继承了RecordableObject的类，并单独记录hero，注意hero在列表中会被重复记录
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
                dos.CaptureState(recordableObject);//此处不会有物体被摧毁，所以不会空指针
                dynamicSceneRecode.Add(dos);
            }
        }
    }
    public void LoadSavedScene()
    {//会加载除hero之外的所有物体
        foreach (StaticObjectState state in staticSceneRecode)
        {
            if (state.tag != "Player") 
            {
                state.RestoreState(state.recordableObject);
            }
        }
        //去除新生成的物体
        foreach (RecordableObject recordableObject in GameObject.FindObjectsOfType<RecordableObject>())
        {
            if (recordableObject.objectState.recordableObject!=recordableObject)
            {//该物体未被记录
                Debug.Log("destory " + (recordableObject.name));
                Destroy(recordableObject.gameObject);
            }
        }
        foreach (DynamicObjectState state in dynamicSceneRecode)
        {
            //恢复被删除和改变的物体
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
