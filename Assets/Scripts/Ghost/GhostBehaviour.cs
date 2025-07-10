using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Ghost replays recorded key inputs and now interacts with the world the same
/// way the hero does: by “pressing” the <kbd>E</kbd> key.  When an <kbd>E</kbd>
/// command is encountered we look for the nearest <see cref="IInteractable"/>
/// inside a small radius and invoke <c>Interact()</c>.  This automatically works
/// with every existing (and future) interactable object that implements that
/// interface – no more hard‑coding for turrets only.
/// </summary>
public class GhostBehaviour : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 0.1f;

    [Header("Interaction")]
    [Tooltip("Radius used to search for interactables when the ghost \"presses\" E.")]
    [SerializeField] private float interactSearchRadius = 0.5f;

    private Queue<Record> records;
    private bool isReplaying;
    private bool facingLeft = true; // ← 记录当前朝向
    private Shooter shooter;

    #region Public API
    public void StartReplay(Queue<Record> inputRecords)
    {
        records = inputRecords;
        isReplaying = true;
    }
    #endregion

    void Awake()
    {
        shooter = GetComponentInChildren<Shooter>();
    }

    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        BulletBehaviour bullet = collision.GetComponent<BulletBehaviour>();
        if (bullet != null && bullet.isEnemy)
        {
            Debug.Log("Ghost hit by enemy bullet");
        }
    }


    #region Unity callbacks
    private void FixedUpdate()
    {
        if (!isReplaying || records == null || records.Count == 0) return;

        Record frame = records.Dequeue();
        foreach (char key in frame.keys)
        {
            switch (key)
            {
                case 'a':
                    facingLeft = true;
                    transform.position += DirFromKey(key) * speed;
                    break;

                case 'd':
                    facingLeft = false;
                    transform.position += DirFromKey(key) * speed;
                    break;

                case 'w':
                case 's':
                    TryUseStairs(key == 'w');
                    break;

                case 'e':
                    TryInteract();
                    break;

                case 'j':
                    shooter.faceLeft = facingLeft;
                    shooter.Fire();
                    break;
            }
            Vector3 s = transform.localScale;
            float targetSign = facingLeft ? -1f : 1f;          // prefab 默认朝右；如默认朝左就反过来
            if (Mathf.Sign(s.x) != targetSign)                 // 只在朝向改变时改一次
            {
                s.x = Mathf.Abs(s.x) * targetSign;
                transform.localScale = s;
            }
}


        if (records.Count == 0)
        {
            Destroy(gameObject);
            GameManager.Instance.OnGhostFinished();
        }
    }
    #endregion

    #region Helpers – movement
    private static Vector3 DirFromKey(char key) => key switch
    {
        'a' => Vector3.left,
        'd' => Vector3.right,
        _ => Vector3.zero
    };
    #endregion

    #region Helpers – interaction
    /// <summary>
    /// Mimic hero pressing E by interacting with the nearest IInteractable.
    /// </summary>
    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactSearchRadius);
        IInteractable nearest = null;
        float minDist = float.MaxValue;

        foreach (var h in hits)
        {
            var interactable = h.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float d = Vector2.Distance(interactable.GetTransform().position, transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = interactable;
            }
        }

        nearest?.Interact();
    }
    #endregion

    #region Helpers – stairs
    private void TryUseStairs(bool commandUp)
    {
        // Very small radius because we must already be on the stair trigger.
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.05f);
        foreach (var h in hits)
        {
            StairsBehaviour stairs = h.GetComponent<StairsBehaviour>();
            if (stairs != null)
            {
                stairs.TryTeleport(transform, commandUp);
                break; // Only need one stair per frame.
            }
        }
    }
    #endregion
}
