using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class KeyRecorder : MonoBehaviour
{
    [Header("auto update")]
    public static List<OperationType> currentInput;
    public bool isRecording = false;
    public List<List<OperationType>> keyboardSequence;
    
    static bool isPressedUp = false;
    static bool isPressdInteracte = false;
    public bool lastRecording;
    // Start is called before the first frame update
    void Start()
    {
        keyboardSequence = new List<List<OperationType>>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        getKeyInputs();
        if (lastRecording^isRecording&&isRecording)
        {//上升沿,开始记录时清楚上一次的记录
            keyboardSequence.Clear();
        }
        lastRecording = isRecording;
        if (isRecording)
        {
            keyboardSequence.Add(CollectKeyInputs());
            //Debug.Log("Adding keys" + keyboardSequence.Count);
        }
        
    }
    public static List<OperationType> CollectKeyInputs()
    {//统一处理输入
        return currentInput;
    }
    private static void getKeyInputs()
    {//该方法一帧只能调用一次
        currentInput = new List<OperationType>();
        if (Input.GetKey(KeyCode.A)) currentInput.Add(OperationType.LEFT);
        if (Input.GetKey(KeyCode.D)) currentInput.Add(OperationType.RIGHT);

        if (Input.GetKey(KeyCode.W) ^ isPressedUp && Input.GetKey(KeyCode.W)) currentInput.Add(OperationType.UP);
        isPressedUp = Input.GetKey(KeyCode.W);//在识别按键“上”时，只识别上升沿

        if (Input.GetKey(KeyCode.S)) currentInput.Add(OperationType.DOWN);

        if (Input.GetKey(KeyCode.E)^ isPressdInteracte&&Input.GetKey(KeyCode.E)) currentInput.Add(OperationType.INTERACTE);
        isPressdInteracte = Input.GetKey(KeyCode.E);//在识别互动时，仅识别上升沿
    }
    public enum OperationType{
        UP,DOWN, LEFT, RIGHT,
        INTERACTE,
        SHOOT
    }
}
