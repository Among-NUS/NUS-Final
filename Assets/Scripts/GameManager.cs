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
        if (Input.GetKeyDown(KeyCode.U)) StartRecording();
        if (Input.GetKeyDown(KeyCode.I)) StopAndReplay();
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

    public void StopAndReplay()
    {
        if (!isRecording) return;

        isRecording = false;

        if (previewGhost != null)
        {
            Destroy(previewGhost);
            previewGhost = null;
        }

        // ��������
        cooldownBar.StopRecording();

        GameObject ghost = Instantiate(ghostPrefab, snapshot.position, Quaternion.identity);
        GhostBehaviour ghostScript = ghost.GetComponent<GhostBehaviour>();
        ghostScript.StartReplay(new Queue<Record>(inputRecords));

        // ��������״̬
        isGhostActive = true;

        Debug.Log("ֹͣ¼�ƣ���ʼ�ط�");
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
        Debug.Log("����طŽ���");
    }
}