using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject ghostPrefab;
    public GameObject previewGhostPrefab;
    public CooldownBarBehaviour cooldownBar;

    GameObject previewGhost;
    Snapshot snapshot;
    Queue<Record> inputRecords = new();

    bool isRecording = false;
    bool isGhostActive = false;  // 是否有幽灵在活动（穿越状态）

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
        if (Input.GetKeyDown(KeyCode.U)) StartRecording();
        if (Input.GetKeyDown(KeyCode.I)) StopAndReplay();
    }

    void FixedUpdate()
    {
        if (cooldownBar == null) return;

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
        else
        {
            // 非录制时恢复能量（包括幽灵存在时）
            cooldownBar.RegenerateEnergy();
        }
    }

    public void StartRecording()
    {
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

        previewGhost = Instantiate(previewGhostPrefab, snapshot.position, Quaternion.identity);
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

        GameObject ghost = Instantiate(ghostPrefab, snapshot.position, Quaternion.identity);
        GhostBehaviour ghostScript = ghost.GetComponent<GhostBehaviour>();
        ghostScript.StartReplay(new Queue<Record>(inputRecords));

        // 设置幽灵活动状态
        isGhostActive = true;

        Debug.Log("停止录制，开始回放");
    }

    void CancelRecording()
    {
        isRecording = false;

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
        Debug.Log("幽灵回放结束");
    }
}