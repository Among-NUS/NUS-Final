using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{
    public enum GamePhase { Normal, Recording, TimeStop }

    public static GameManager Instance { get; private set; }
    public GamePhase currentPhase = GamePhase.Normal;

    public GameObject ghostPrefab;
    public GameObject previewGhostPrefab;
    public CooldownRingBehaviour cooldownRing;

    GameObject previewGhost;
    Snapshot snapshot;
    Queue<Record> inputRecords = new();

    public MessageDisplay messageDisplay;
    public bool IsRecording => currentPhase == GamePhase.Recording;

    void Awake()
    {
        snapshot = new Snapshot();

        ghostPrefab        = Resources.Load<GameObject>("Prefabs/GhostPrefab");
        previewGhostPrefab = Resources.Load<GameObject>("Prefabs/PreviewGhostPrefab");

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // ─────────────────────────────────────────────────────────────
    //  键盘入口：U 开始录制，I 结束并回放，O 结束并时间静止
    // ─────────────────────────────────────────────────────────────
    void Update()
    {
        if (Time.timeScale == 0) return;
        if (Input.GetKeyDown(KeyCode.U) && currentPhase == GamePhase.Normal)
            StartRecording();
        else if (Input.GetKeyDown(KeyCode.I) && currentPhase == GamePhase.Recording)
            StopAndReplay();
        else if (Input.GetKeyDown(KeyCode.O) && currentPhase == GamePhase.Recording)
            StopRecordingAndFreeze();
    }

    //────────────────────────  录制流程  ────────────────────────
    public void StartRecording()
    {
        if (!cooldownRing.CanStartRecording)
        {
            Debug.Log("能量不足，无法开始录制");
            return;
        }

        if (GameObject.FindGameObjectsWithTag("Ghost").Length != 0)
        {
            Debug.Log("场上已有Ghost，无法开始录制");
            return;
        }

        currentPhase = GamePhase.Recording;
        inputRecords.Clear();

        snapshot = new Snapshot();
        snapshot.Capture();

        if (previewGhost) Destroy(previewGhost);
        previewGhost = Instantiate(previewGhostPrefab, snapshot.playerPosition, Quaternion.identity);

        cooldownRing.StartRecording();
        Debug.Log("开始录制");
    }

    public void StopAndReplay()
    {
        if (currentPhase != GamePhase.Recording) return;

        snapshot.Restore();             // 回到录制起点
        currentPhase = GamePhase.Normal;

        cooldownRing.StopRecording();
        BeginReplay();
    }

    public void StopRecordingAndFreeze()
    {
        if (currentPhase != GamePhase.Recording) return;

        snapshot.Restore();
        currentPhase = GamePhase.TimeStop;

        cooldownRing.StopRecording();
    }

    // 现在从 CooldownRingBehaviour 调用 → 需要 public
    public void CancelRecording()
    {
        currentPhase = GamePhase.Normal;

        if (previewGhost)
        {
            Destroy(previewGhost);
            previewGhost = null;
        }

        inputRecords.Clear();
        cooldownRing.CancelRecording();
        Debug.Log("录制被取消");
    }

    //────────────────────────  幽灵播放  ────────────────────────
    public void RecordKeyInput(List<char> keys)
    {
        if (currentPhase == GamePhase.Recording)
            inputRecords.Enqueue(new Record(keys));
    }

    public void OnGhostFinished()
    {
        currentPhase = GamePhase.Normal;
        Debug.Log("幽灵回放结束");
    }

    public void BeginReplay()
    {
        currentPhase = GamePhase.Normal;

        if (previewGhost)
        {
            Destroy(previewGhost);
            previewGhost = null;
        }

        GameObject ghost = Instantiate(ghostPrefab, snapshot.playerPosition, Quaternion.identity);
        ghost.GetComponent<GhostBehaviour>()
             .StartReplay(new Queue<Record>(inputRecords));   // 深拷贝一份指令
    }
}
