using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeReverseController : MonoBehaviour
{//统一时间回溯时影响的单位的行为,组合sceneRecorder使用
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
            Debug.LogError("时间管理器没有记录组件！");
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
            //同步主角和ghost的位置,速度
            ghostPrefab.transform.position = GetComponent<SceneRecorder>().heroState.position;
            ghostPrefab.GetComponent<Rigidbody2D>().velocity = (Vector2)GetComponent<SceneRecorder>().heroState.extraData["velocity"];
        }
        isPressedLoad = Input.GetKey(KeyCode.O);
    }
}
