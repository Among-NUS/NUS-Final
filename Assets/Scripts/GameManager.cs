using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject ghostPrefab;
    public GameObject previewGhostPrefab;

    GameObject previewGhost;

    Snapshot snapshot;
    Queue<Record> inputRecords = new();

    bool isRecording = false;
    public bool IsRecording => isRecording;

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

    public void StartRecording()
    {
        inputRecords.Clear();
        isRecording = true;

        GameObject hero = GameObject.FindWithTag("Player");
        if (hero != null)
            snapshot = new Snapshot(hero.transform.position);

        if (previewGhost != null)
            Destroy(previewGhost);

        previewGhost = Instantiate(previewGhostPrefab, snapshot.position, Quaternion.identity);
    }

    public void StopAndReplay()
    {
        isRecording = false;

        if (previewGhost != null)
        {
            Destroy(previewGhost);
            previewGhost = null;
        }

        GameObject ghost = Instantiate(ghostPrefab, snapshot.position, Quaternion.identity);
        GhostBehaviour ghostScript = ghost.GetComponent<GhostBehaviour>();
        ghostScript.StartReplay(new Queue<Record>(inputRecords)); // Éî¿½±´
    }

    public void RecordKeyInput(List<char> keys)
    {
        inputRecords.Enqueue(new Record(keys));
    }
}
