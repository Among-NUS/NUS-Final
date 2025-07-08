using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum GamePhase { Normal, Recording, TimeStop, Replaying }

    public static GameManager Instance { get; private set; }
    public GamePhase currentPhase = GamePhase.Normal;

    public GameObject ghostPrefab;
    public GameObject previewGhostPrefab;
    public CooldownBarBehaviour cooldownBar;

    GameObject previewGhost;
    Snapshot snapshot;
    Queue<Record> inputRecords = new();

    bool isRecording = false;
    bool isGhostActive = false;  // 是否有幽灵在活动（穿越状态）

    bool travellingMode = false;

    public bool IsRecording => isRecording;
    public bool IsGhostActive => isGhostActive;

    void Awake()
    {
        ghostPrefab = Resources.Load<GameObject>("Prefabs/GhostPrefab");
        previewGhostPrefab = Resources.Load<GameObject>("Prefabs/PreviewGhostPrefab");

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && currentPhase == GamePhase.Normal) travellingMode = !travellingMode;

        if (Input.GetKeyDown(KeyCode.U) && currentPhase == GamePhase.Normal) StartRecording();
        
        if (Input.GetKeyDown(KeyCode.I) && currentPhase == GamePhase.Recording)
        {
            if (travellingMode) StopRecordingAndFreeze();
            else StopAndReplay();

            SimpleScaleAnimation scaleAnim = GetComponent<SimpleScaleAnimation>();
            if (scaleAnim != null)
            {
                scaleAnim.PlayAnimation();
            }
        }
        if (currentPhase == GamePhase.TimeStop)
        {
            cooldownBar.ConsumeEnergyForTravel();
            if (cooldownBar.IsEnergyDepleted)
            {
                BeginReplay();
            }
        }        
    }

        if (isRecording)
        {
            // 录制时，增加使用的能量
            cooldownBar.TickRecording();

            // 检查能量是否耗尽
            if (cooldownBar.IsRecordingEnergyDepleted())
            {
                CancelRecording();
                Debug.Log("能量耗尽，录制被取消");
            }
        }
        else if(currentPhase == GamePhase.Normal|| currentPhase == GamePhase.Replaying)
        {
            // 非录制时恢复能量（包括幽灵存在时）
            cooldownBar.RegenerateEnergy();
        }
    }

    public void StartRecording()
    {
        currentPhase = GamePhase.Recording;
        if (!cooldownBar.CanStartRecording)
        {
            Debug.Log("能量不足，无法开始录制");
            return;
        }

        inputRecords.Clear();
        isRecording = true;

        GameObject hero = GameObject.FindWithTag("Player");
        if (hero != null)
            snapshot = new Snapshot(hero.transform.position);

        if (previewGhost != null)
            Destroy(previewGhost);

        previewGhost = Instantiate(previewGhostPrefab, snapshot.playerPosition, Quaternion.identity);
        cooldownBar.StartRecording();

        Debug.Log("开始录制");
    }

    public void StopAndReplay()
    {
        if (!isRecording) return;

        isRecording = false;

        if (previewGhost != null)
        {
            Destroy(previewGhost);
            previewGhost = null;
        }

        // 消耗能量
        cooldownBar.StopRecording();

        GameObject ghost = Instantiate(ghostPrefab, snapshot.playerPosition, Quaternion.identity);
        GhostBehaviour ghostScript = ghost.GetComponent<GhostBehaviour>();
        ghostScript.StartReplay(new Queue<Record>(inputRecords));

        // 设置幽灵活动状态
        isGhostActive = true;

        snapshot.Restore();

        Debug.Log("停止录制，开始回放");
    }

    public void StopRecordingAndFreeze()
    {
        if (currentPhase != GamePhase.Recording) return;

        isRecording = false;
        currentPhase = GamePhase.TimeStop;
        //if (previewGhost != null)
        //{
        //    Destroy(previewGhost);
        //    previewGhost = null;
        //}

        // 消耗能量
        cooldownBar.StopRecording();

        //GameObject ghost = Instantiate(ghostPrefab, snapshot.position, Quaternion.identity);
        //GhostBehaviour ghostScript = ghost.GetComponent<GhostBehaviour>();
        //ghostScript.StartReplay(new Queue<Record>(inputRecords));

        // 设置幽灵活动状态
        //isGhostActive = true;

        snapshot.Restore();

        Debug.Log("ֹͣ停止录制,进入timestop");
    }

    void CancelRecording()
    {
        isRecording = false;
        currentPhase = GamePhase.Normal;

        if (previewGhost != null)
        {
            Destroy(previewGhost);
            previewGhost = null;
        }

        inputRecords.Clear();
        cooldownBar.CancelRecording();
        Debug.Log("录制被取消");
    }

    public void RecordKeyInput(List<char> keys)
    {
        if (!isRecording) return;
        inputRecords.Enqueue(new Record(keys));
    }

    /// <summary>
    /// 当幽灵结束回放时调用
    /// </summary>
    public void OnGhostFinished()
    {
        isGhostActive = false;
        currentPhase = GamePhase.Normal;
        Debug.Log("幽灵回放结束");
    }
    
    public void BeginReplay()
    {                   
        currentPhase = GamePhase.Replaying;
        if (previewGhost != null)
        {
            Destroy(previewGhost);
            previewGhost = null;
        }
        // 在 snapshot 处生成幽灵
        GameObject ghost = Instantiate(ghostPrefab, snapshot.playerPosition, Quaternion.identity);
        ghost.GetComponent<GhostBehaviour>().StartReplay(new Queue<Record>(inputRecords)); // 把已录指令给幽灵

        isGhostActive = true;
        Debug.Log("能量耗尽 → 开始 Ghost 回放");
    }
}