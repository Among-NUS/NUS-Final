using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeReverseController : MonoBehaviour
{//ͳһʱ�����ʱӰ��ĵ�λ����Ϊ,���sceneRecorderʹ��
    [Header("need to drag")]
    public GameObject ghostPrefab;
    [Header("auto update")]
    public SceneRecorder mySceneRecorder;

    bool isPressedSave = false;
    bool isPressedLoad = false;
    // Start is called before the first frame update
    void Start()
    {
        mySceneRecorder = GetComponent<SceneRecorder>();
        if (mySceneRecorder==null)
        {
            Debug.LogError("ʱ�������û�м�¼�����");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.I) ^ isPressedSave && Input.GetKey(KeyCode.I))
        {
            GetComponent<SceneRecorder>().RecordCurrentScene();
            Debug.Log("saved!");
            ghostPrefab.GetComponent<KeyRecorder>().isRecording = true;
        }
        isPressedSave = Input.GetKey(KeyCode.I);
        if (Input.GetKey(KeyCode.O) ^ isPressedLoad && Input.GetKey(KeyCode.O))
        {
            GetComponent<SceneRecorder>().LoadSavedScene();
            Debug.Log("loaded!");
            ghostPrefab.GetComponent<KeyRecorder>().isRecording = false;
            ghostPrefab.GetComponent<Ghost>().isReplay = true;
            //ͬ�����Ǻ�ghost��λ��,�ٶ�
            ghostPrefab.transform.position = GetComponent<SceneRecorder>().heroState.position;
            ghostPrefab.GetComponent<Rigidbody2D>().velocity = (Vector2)GetComponent<SceneRecorder>().heroState.extraData["velocity"];
        }
        isPressedLoad = Input.GetKey(KeyCode.O);
    }
}
