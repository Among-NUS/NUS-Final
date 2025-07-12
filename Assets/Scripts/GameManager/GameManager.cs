using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum GamePhase { Normal, Recording, TimeStop, Replaying }

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

        ghostPrefab = Resources.Load<GameObject>("Prefabs/GhostPrefab");
        previewGhostPrefab = Resources.Load<GameObject>("Prefabs/PreviewGhostPrefab");

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U) && currentPhase == GamePhase.Normal) StartRecording();
        if (Input.GetKeyDown(KeyCode.I) && currentPhase == GamePhase.Recording) StopRecordingAndFreeze();
        if (Input.GetKeyDown(KeyCode.O) && currentPhase == GamePhase.Recording)  StopAndReplay();
    }

    void FixedUpdate()
    {
        if (cooldownRing == null) return;

        if (currentPhase == GamePhase.TimeStop)
        {
            cooldownRing.ConsumeEnergyForTravel();
            if (cooldownRing.IsEnergyDepleted)
            {
                BeginReplay();
            }
        }

        if (currentPhase == GamePhase.Recording)
        {
            // 录制时，增加使用的能量
            cooldownRing.TickRecording();

            // 检查能量是否耗尽
            if (cooldownRing.IsRecordingEnergyDepleted())
            {
                CancelRecording();
                Debug.Log("能量耗尽，录制被取消");
            }
        }
        else if (currentPhase == GamePhase.Normal)
        {
            // 非录制时恢复能量（包括幽灵存在时）
            cooldownRing.RegenerateEnergy();
        }
    }

    public void GameOver()
    {

    }

    public void StartRecording()
    {
        if (!cooldownRing.CanStartRecording)
        {
            Debug.Log("能量不足，无法开始录制");
            return;
        }

        currentPhase = GamePhase.Recording;
        inputRecords.Clear();

        snapshot = new Snapshot();
        snapshot.Capture();

        if (previewGhost != null)
            Destroy(previewGhost);

        previewGhost = Instantiate(previewGhostPrefab, snapshot.playerPosition, Quaternion.identity);
        cooldownRing.StartRecording();

        Debug.Log("开始录制");
    }
    public void StopAndReplay()
    {
        if (currentPhase != GamePhase.Recording) return;

        //messageDisplay.ShowMessageForOneSecond();

        snapshot.Restore();
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

    void CancelRecording()
    {
        currentPhase = GamePhase.Normal;

        if (previewGhost != null)
        {
            Destroy(previewGhost);
            previewGhost = null;
        }

        inputRecords.Clear();
        cooldownRing.CancelRecording();
        Debug.Log("录制被取消");
    }

    public void RecordKeyInput(List<char> keys)
    {
        if (currentPhase != GamePhase.Recording) return;
        inputRecords.Enqueue(new Record(keys));
    }

    /// <summary>
    /// 当幽灵结束回放时调用
    /// </summary>
    public void OnGhostFinished()
    {
        currentPhase = GamePhase.Normal;
        Debug.Log("幽灵回放结束");
    }

    public void BeginReplay()
    {
        currentPhase = GamePhase.Normal;
        if (previewGhost != null)
        {
            Destroy(previewGhost);
            previewGhost = null;
        }
        // 在 snapshot 处生成幽灵
        GameObject ghost = Instantiate(ghostPrefab, snapshot.playerPosition, Quaternion.identity);
        ghost.GetComponent<GhostBehaviour>().StartReplay(new Queue<Record>(inputRecords)); // 把已录指令给幽灵

    }
}