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
    bool isGhostActive = false;  // �Ƿ��������ڻ����Խ״̬��

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
        if (Input.GetKeyDown(KeyCode.U) && currentPhase == GamePhase.Normal) StartRecording();
        if (Input.GetKeyDown(KeyCode.I) && currentPhase == GamePhase.Recording) StopRecordingAndFreeze();
        if (currentPhase == GamePhase.TimeStop)
        {
            cooldownBar.ConsumeEnergyForTravel();
            if (cooldownBar.IsEnergyDepleted)
            {
                BeginReplay();
            }
        }
    }

    void FixedUpdate()
    {
        if (cooldownBar == null) return;

        if (isRecording)
        {
            // ¼��ʱ������ʹ�õ�����
            cooldownBar.TickRecording();

            // ��������Ƿ�ľ�
            if (cooldownBar.IsRecordingEnergyDepleted())
            {
                CancelRecording();
                Debug.Log("�����ľ���¼�Ʊ�ȡ��");
            }
        }
        else
        {
            // ��¼��ʱ�ָ������������������ʱ��
            cooldownBar.RegenerateEnergy();
        }
    }

    public void StartRecording()
    {
        currentPhase = GamePhase.Recording;
        if (!cooldownBar.CanStartRecording)
        {
            Debug.Log("�������㣬�޷���ʼ¼��");
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

        Debug.Log("��ʼ¼��");
    }

    public void StopRecordingAndFreeze()
    {
        if (currentPhase != GamePhase.Recording) return;

        isRecording = false;
        currentPhase = GamePhase.TimeStop;
        Time.timeScale = 0f;//进入时停，暂停全局时间
        if (previewGhost != null)
        {
            Destroy(previewGhost);
            previewGhost = null;
        }

        // ��������
        cooldownBar.StopRecording();

        //GameObject ghost = Instantiate(ghostPrefab, snapshot.position, Quaternion.identity);
        //GhostBehaviour ghostScript = ghost.GetComponent<GhostBehaviour>();
        //ghostScript.StartReplay(new Queue<Record>(inputRecords));

        // ��������״̬
        //isGhostActive = true;

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
        Debug.Log("¼�Ʊ�ȡ��");
    }

    public void RecordKeyInput(List<char> keys)
    {
        if (!isRecording) return;
        inputRecords.Enqueue(new Record(keys));
    }

    /// <summary>
    /// ����������ط�ʱ����
    /// </summary>
    public void OnGhostFinished()
    {
        isGhostActive = false;
        currentPhase = GamePhase.Normal;
        Debug.Log("����طŽ���");
    }
    
    public void BeginReplay()
    {
        Time.timeScale = 1f;                               // 恢复时间
        currentPhase = GamePhase.Replaying;

        // 在 snapshot 处生成幽灵
        GameObject ghost = Instantiate(ghostPrefab, snapshot.position, Quaternion.identity);
        ghost.GetComponent<GhostBehaviour>().StartReplay(new Queue<Record>(inputRecords)); // 把已录指令给幽灵

        isGhostActive = true;
        Debug.Log("能量耗尽 → 开始 Ghost 回放");
    }
}