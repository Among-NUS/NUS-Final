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
    public float jumpForce = 5f;

    [Header("Interaction")]
    [Tooltip("Radius used to search for interactables when the ghost \"presses\" E.")]
    [SerializeField] private float interactSearchRadius = 0.5f;
    private Animator ghostAnimator;
    public float layerChangeCooldown = 0.8f;
    private float lastCooldown;
    public int nearLayer = 100;
    public int farLayer = -1;
    [SerializeField] private SpriteRenderer ghostSR;

    private Queue<Record> records;
    private bool isReplaying;
    private bool facingLeft = true; // ← 记录当前朝向
    private Rigidbody2D ghostRigidBody;
    private Shooter shooter;
    private bool isWalking = false; // ← 用于记录是否在行走
    private bool isOnGround = true;
    private bool isJumping = false;
    private int groundLayer = 13;
    private GameObject Elevator = null;
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
        ghostAnimator = GetComponent<Animator>();
        ghostSR = GetComponent<SpriteRenderer>();
        ghostRigidBody = GetComponent<Rigidbody2D>();
        Debug.Assert(shooter != null, "Shooter component is not assigned.");
        Debug.Assert(ghostAnimator != null, "Ghost animator is not assigned.");
    }

    void Start()
    {
        ghostAnimator = GetComponent<Animator>();
        Debug.Assert(ghostAnimator != null, "Ghost animator is not assigned.");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.GetComponent<StairsBehaviour>() != null)
        {
            Debug.Log("ghost found elevator");
            Elevator = collision.gameObject;
        }
        BulletBehaviour bullet = collision.GetComponent<BulletBehaviour>();
        if (bullet != null && bullet.isEnemy)
        {
            Debug.Log("Ghost hit by enemy bullet");
        }
    }


    #region Unity callbacks
    private bool lastFramePressedE = false; // ← 新增字段，放在类里

    private void FixedUpdate()
    {

        if (!isReplaying || records == null || records.Count == 0) return;
        if (Time.time - lastCooldown > layerChangeCooldown)
        {
            setLayerNear();
        }
        Record frame = records.Dequeue();
        ghostAnimator.SetBool("isJumping", ghostRigidBody.velocity.y > 0);
        ghostAnimator.SetBool("isFalling", ghostRigidBody.velocity.y < 0);
        bool pressedE = false;
        ghostAnimator.SetBool("isWalking", false); // 重置行走状态
        foreach (char key in frame.keys)
        {

            switch (key)
            {
                case 'a':
                    facingLeft = true;
                    transform.position += DirFromKey(key) * speed;
                    ghostAnimator.SetBool("isWalking", true);
                    break;

                case 'd':
                    facingLeft = false;
                    transform.position += DirFromKey(key) * speed;
                    ghostAnimator.SetBool("isWalking", true);
                    break;

                case 'w':
                case 's':
                    Debug.Log("ghost tring to use stairs");
                    TryUseStairs(key == 'w');
                    break;

                case 'e':
                    pressedE = true; // 标记这帧按下了 e
                    break;

                case 'j':
                    if (shooter.CanFire())
                    {
                        shooter.faceLeft = facingLeft;
                        shooter.Fire();
                        ghostAnimator.SetTrigger("shootAnim");
                    }
                    break;
                case ' ':
                    if (isOnGround)
                    {
                        ghostRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        isOnGround = false;
                        isJumping = true;
                    }
                    break;
            }
        }

        // 控制 sprite 方向镜像翻转
        Vector3 s = transform.localScale;
        float targetSign = facingLeft ? -1f : 1f;
        if (Mathf.Sign(s.x) != targetSign)
        {
            s.x = Mathf.Abs(s.x) * targetSign;
            transform.localScale = s;
        }

        // 只在 e 第一次按下的那一帧执行交互
        if (pressedE && !lastFramePressedE)
        {
            TryInteract();
        }
        lastFramePressedE = pressedE;

        // 回放结束时销毁自己
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
        if (Elevator == null)
        {
            return;
        }
        else
        {
            StairsBehaviour elevatorScript = Elevator.GetComponent<StairsBehaviour>();
            Debug.Log("ghost using exist elevator");
            elevatorScript.TryTeleport(transform, commandUp);
        }
        // Very small radius because we must already be on the stair trigger.
        /*Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (var h in hits)
        {
            Debug.Log("ghost looking at " + h.gameObject.name);
        }
        foreach (var h in hits)
        {
            Debug.Log("ghost looking at " + h.gameObject.name);
            StairsBehaviour stairs = h.GetComponent<StairsBehaviour>();
            if (stairs != null)
            {
                Debug.Log("ghost using stair" + h.gameObject.name);
                stairs.TryTeleport(transform, commandUp);
                break; // Only need one stair per frame.
            }
        }*/
    }
    #endregion

    public void setLayerFar()
    {
        Debug.Log("Ghost ser layer far");
        ghostSR.sortingOrder = farLayer;
        lastCooldown = Time.time;
    }

    public void setLayerNear()
    {
        ghostSR.sortingOrder = nearLayer;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer) // Check if collision is with the ground layer
        {
            isOnGround = true;  // Reset grounded state when hitting the ground
            isJumping = false;  // Reset jumping state when on the ground


        }

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer) // Check if collision is with the ground layer
        {
            isOnGround = false; // Set isOnGround to false when exiting the ground layer
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        BulletBehaviour bullet = collision.GetComponent<BulletBehaviour>();
        if (bullet != null && bullet.isEnemy)
        {
            Debug.Log("Hero hit by enemy bullet");
            FindObjectOfType<GameOverUI>().ShowGameOver();
        }

        if (collision.GetComponent<StairsBehaviour>() != null)
        {
            Debug.Log("ghost left elevator");
            Elevator = null;
        }
    }
}
